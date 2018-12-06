# PAKtA
Press Any Key to Abort – cmd timeout, pause, sleep, choice alternative

CMD batch alternative to commands like timeout. 
If there is a need to abort/close/stop operation by pressing any key pakta will do it.

Usage
Press any key to INTERRUPT (not continue) batch task while timeout goes.
If no key is pressed during timeout, batch will continue normally.

Errorlevels:
0 if timeout or not specified key pressed.
1 if specified key pressed.
-1 arguments error
-2 general error

pakta.exe -T timeout ms (-I key to continue) (-X KILL PARENT PROCESS instead of generating errorlevel)
