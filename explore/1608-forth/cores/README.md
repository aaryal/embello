These firmware images are copies from the [Mecrisp-Stellaris][MG] repository on
GitHub.

The builds are used in the Embello project, and contain a few small extensions
to Matthias Koch's official [Mecrisp][MS] builds on SourceForge:

* the UART setup and welcome text is now performed by a new `init` word, defined
  in the core - this allows overriding it, and avoiding USART initialisation
  when not needed
* the binary also contains a second `init` at the end, which calls `eraseflash`
  to make sure all remaining flash memory has been cleared and to avoid a
  problem when Mecrisp has been loaded into a chip with non-empty flash - the
  effect is a full erase on startup, which then also erases this `init` itself
* interrupts are disabled before erasing flash memory, since the code always
  ends in a software reset - this prevents some crashes during the erase process
* adds a `forgetram` word to clear the RAM-based dictionary - this helps with
  USB-based configurations, to reset RAM without re-enumerating the USB bus

All these images use the Register Allocator (RA) variant of Mecrisp, which takes
up the lower 20 KB of flash memory. If you have a board with this code loaded
and other stuff on top, you can always revert to these "standard" binaries with
the following command:

    $5000 eraseflashfrom

This will work even when `erasflash` itself has been redefined.

With a big thank you to Matthias for creating and supporting [Mecrisp][MG] in
the first place, but also for very generously offering to implement all the
above changes. The source code for these modified builds can be found in the
"spezial" branch of Mecrisp on [GitHub][MG].

   [MG]: https://github.com/jeelabs/mecrisp-stellaris
   [MS]: http://mecrisp.sourceforge.net
