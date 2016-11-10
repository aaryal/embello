\ support for fow-power sleep

     RCC $48 + constant RCC-APB1SMENR
     RCC $50 + constant RCC-CSR

$40007C00 constant LPTIM1
   LPTIM1 $00 + constant LPTIM-ISR
   LPTIM1 $04 + constant LPTIM-ICR
   LPTIM1 $08 + constant LPTIM-IER
   LPTIM1 $0C + constant LPTIM-CFGR
   LPTIM1 $10 + constant LPTIM-CR
   LPTIM1 $14 + constant LPTIM-CMP
   LPTIM1 $18 + constant LPTIM-ARR
   LPTIM1 $1C + constant LPTIM-CNT

$40007000 constant PWR
      PWR $0 + constant PWR-CR
      PWR $4 + constant PWR-CSR

$40010400 constant EXTI
     EXTI $00 + constant EXTI-IMR
     EXTI $04 + constant EXTI-EMR
\    EXTI $08 + constant EXTI-RTSR
\    EXTI $0C + constant EXTI-FTSR
\    EXTI $10 + constant EXTI-SWIER
     EXTI $14 + constant EXTI-PR

\ see https://developer.arm.com/docs/dui0662/latest/4-cortex-m0-peripherals/
\                       43-system-control-block/436-system-control-register
$E000ED10 constant SCR

: lptim? ( -- )
  LPTIM1
  cr  ." ISR " dup @ h.2 space 4 +
      ." ICR " dup @ h.2 space 4 +
      ." IER " dup @ h.2 space 4 +
     ." CFGR " dup @ hex.      4 +
       ." CR " dup @ h.2 space 4 +
      ." CMP " dup @ h.4 space 4 +
      ." ARR " dup @ h.4 space 4 +
      ." CNT " dup @ h.4 space drop ;

: +lptim
  0 bit RCC-CSR bis!              \ set LSION
  begin 1 bit RCC-CSR bit@ until  \ wait for LSIRDY
  %01 18 lshift RCC-CCIPR bis!    \ use LSI clock
  31 bit RCC-APB1ENR bis!         \ enable LPTIM1
  31 bit RCC-APB1SMENR bis!       \ also enable in sleep mode
  %111 9 lshift LPTIM-CFGR !      \ 128 prescaler
  0 bit LPTIM-CR bis!             \ set ENABLE
  37000 128 / LPTIM-ARR !         \ 1s timout
;

: wfe ( -- ) [ $BF20 h, ] inline ; \ WFE Opcode, enters sleep mode

: stop1s ( -- )
\ 1 bit PWR-CR bis!                     \ set PDDS for standby mode
\ 0 bit PWR-CR bis!                     \ set LPSDSR
  1 bit LPTIM-CR bis!                   \ set SNGSTRT
  1 bit LPTIM-IER bis!                  \ set ARRMIE
\ 29 bit EXTI-IMR bic!                  \ clear IM29
  29 bit EXTI-EMR bis!                  \ set EM29
\ -1 EXTI-PR !                          \ clear all pending
  2 bit SCR bis!                        \ set SLEEPDEEP
  begin wfe 1 bit LPTIM-ISR bit@ until  \ wait for ARRM
  1 bit LPTIM-ICR bis!                  \ clear ARRM
;

\ : lp-blink ( -- )  only-msi  begin  stop1s led iox!  key? until ;
\
\ rf69-init rf-sleep
\ led-off 2.1MHz
\ 1000 systick-hz  \ FIXME if omitted, blink startup stalls for several seconds
\ +lptim 