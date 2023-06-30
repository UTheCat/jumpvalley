# The Player's Camera

Just like in Juke's Towers of Hell and most other Roblox games, the player's camera should be able to be controlled in three modes:

- First person
- Third person without Shift-Lock
- Third person with Shift-Lock

Here's a rough outline of how this would be implemented.

## Shift-Lock

Shift-Lock, also known as Camera-Lock or Mouse-Lock, is a type of third-person camera that makes the player's character always face in the direction that the camera is facing. If the player is playing with a trackpad or mouse, their cursor gets locked at the center of their screen while Shift-Lock is enabled.

In Jumpvalley, this is toggled by pressing Left-Shift on a keyboard. If Jumpvalley ends up supporting mobile, there will be a button on the user's screen for this on mobile devices too.

## First person

When the camera is in first person, the camera is positioned at the center of the character's head. Just like when Shift-Lock is enabled, the character will always be facing in the same direction the camera is facing when the camera is in first person.

## Third person without Shift-Lock

In third person, the camera is still going to be focused on the camera's head. However, the camera is going to be a certain distance away from the head and can be panned to look at the head from different directions.

## Third person with Shift-Lock

This mode of the camera is the same as third-person without Shift-Lock, except with Shift-Lock enabled. In Juke's Towers of Hell, the camera is shifted a certain distance to the right whenever Shift-Lock is enabled, and there may be an option to toggle this positional shift in Jumpvalley.

## Calculating Rotation and Position

Player input can only change the yaw and pitch angles of the camera. The camera's roll angle is not affected by player input.

Rotation of the camera is based on what the orientation would look like at the center of the object that the camera is focusing on.

In first person, the camera's position in world space is the same as the position of the object that the camera is focusing on.

Third person is achieved by:

1. Rotating a 3D plane based on the yaw, pitch, and roll angles of the camera.
2. Offsetting the Z-coordinate of a point within the rotated 3D plane (in this case, the point is the camera's position)

The yaw, pitch, and roll of the camera should remain unchanged after these two steps. Additionally, you would then have the position of the camera in world space.

