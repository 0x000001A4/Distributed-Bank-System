## How to run

1 - Open the solution with Visual Studio (the solution is inside BoneyBank folder), and build it.  
2 - Right click over PuppetMaster and start a new instance.
All terminals (3 boney and 1 Bank) should be opened.

## What was implemented

- Any number of Boney servers running Paxos (default config files starts up 3 Boneys)
- 1 Bank (must have processID of 4 in the config files since it was hardcoded for this delivery)
- Periodic compareAndSwap requests sent from Bank to Boneys for each Timer Tick
- Perfect Channels
- Logger printing all messages (Proposers, Acceptors, Learners) for each Boney
