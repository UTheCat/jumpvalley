# Character Movement
This is a breakdown of how character movement works (or should work) in Jumpvalley.

Because Jumpvalley is essentially a Juke's Towers of Hell fangame, lots of movement logic borrows from Roblox's handling of character movement.

Thus, the character movement code in Jumpvalley is structured a lot like Roblox's PlayerModule.

Similar to how Roblox does it, there's a base class that's responsible for controlling a character. Some of its components include:

- Directional magnitudes of movement, including:
	- Forward
	- Backward
	- Left
	- Right

<small>-- Notice how there's no upward or downward directional magnitude being accounted for. This is because vertical velocity stuff (such as gravity) is already handled by Godot's engine.</small>

- Climbing logic
- Jumping logic (making a character jump in Godot can be achieved by applying an initial upward velocity).

Subclasses can then build on top of the base class to cater towards a specific type of controller hardware, (keyboard & mouse, touchscreen, controller, etc.).
