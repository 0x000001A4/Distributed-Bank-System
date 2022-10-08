First tier: CLIENTS
Actions: Deposit, Withdraw -> Submitted to (ALL?) bank server processes (2nd tier)

Second tier: BANK SERVERS
Actions: Replicate information (Primary-backup)
Primary is responsible for assigning sequence numbers to requests (using 2PC)
-> Can be frozen

Third tier: BONEY SYSTEM
Actions: Determine primary of bank servers (for given slot)
-> Can be frozen


Paxos is implemented by the group of processes running the Boney SERVICE, responsible for electing a primary bank server.
(Note: Each process in a group is a proposer, acceptor and learner simultaneously)

Every slot (after delta t):

(Pre) Freeze/Unfreeze processes according to config.
1. Select new group leader -> Process with lower ID that is 'not-suspected'
(Note: different replicas may believe different processes to be the leader of the group)
2. Use "Boney" service to elect a primary.