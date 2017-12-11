To run the project:
	1. select the Game manager object in the unity hierarchy
	2. Use the dropdown menu to set the controll type to be qLearnV2
	3. set the Number of Sessions to be the number of sessions that you want the AI to train with before viewing results
	4. Set up the initial framerate variable. This value affects how long the game will pause and wait for movement.
		For actually watching the game play, we suggest using a value at or near 20
		To train the game, set the value somewhere above 100. If the number is above the framerate that Unity is running the game at, the exact value doesn't matter
	5. Set up the number of updates per frame
		For training, setting the number to between 5 and 20 to allow for training to occur quickly. The higher the number, the faster training will go (but performance may take a hit if this number is too large)
		For observing, set this value to 1
	6. Set up the Q learn V2 information variables
		The Q Learn_random_change variable is the change that the snake will take a random action for any given decision.
			We suggest that this value be 0.05 for training, and 0 for observing
		The Q Learn_gamma is the gamma factor that will determine how much the AI values immediate reward vs future reward
			We sugest setting this value to between 0.1 to 0.5
	7. Press the play button to begin training. After the AI has completed its training, the game will automatically pause
	8. To observe:
		Update the Desired Framerate and Updates per frame values to be observing values (20 and 1, respectively)
		set the q Learn_random_change value to 0 and unpause the game by pressing the pause button again.

You can also update the Controll_Type dropdown menu to make the game play with the simple AI, randomly, or by a human