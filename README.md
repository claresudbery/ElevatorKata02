# ElevatorKata01
elevator kata - second attempt 

In this version, I use a LiftEventGenerator class as the single source of events.
This creates an easy way of faking events for test purposes, and means that we don't have to have any Thread.Sleep calls in tests.

I strongly suspect this one is a crazy way of doing things (the code at the moment feels very tortuous), but I'm trying it anyway as a kind of intellectual* exercise.
*self-indulgent

The kata itself (see http://blog.milesburton.com/2013/03/28/elevator-kata-mind-bending-pairing-exercise/):

- An elevator responds to calls containing a source floor and direction
- An elevator delivers passengers to requested floors
- Elevator calls are queued not necessarily FIFO
- You may validate passenger floor requests
- you may implement current floor monitor
- you may implement direction arrow
- you may implement doors (opening and closing)
- you may implement DING!
- there can be more than one elevator
- ?? Max number of lift occupants?

Related links:

http://www.quora.com/Is-there-any-public-elevator-scheduling-algorithm-standard

http://www.quora.com/Why-are-virtually-all-elevator-algorithms-so-inefficient

Paternoster: https://www.youtube.com/watch?v=Ro3Fc_yG3p0

