# Jumpvalley's Display Refresh Rate Cap

Currently, Jumpvalley renders at a maximum of 60 frames per second (FPS). This is to make rendering smooth with player movement.

Player physics are also calculated at 60 FPS. In order for the physics to look smooth on the user's screen, the game must also render at an FPS value that's a multiple of 60.

Simply put, this is because when the game renders at FPS values other than multiples of 60, Godot (the game engine that Jumpvalley was built on) will still calculate physics at 60 frames per second, but it will at many times be out of sync with rendering.

To make 3D rendering and physics look smooth for the most amount of users possible, Jumpvalley's rendering FPS is capped at 60. This is also because:
- The display refresh rate of most monitors is 60 Hz anyways <sup>[1]</sup>. Whenever this is the case, the user won't be able to see the rendering happen faster than 60 times per second since their monitor won't even refresh more than 60 times per second.
- Rendering at framerates higher than 60 is often not a viable option on low-end devices
- Capping the framerate to 60 frames per second allows the framerate to stay consistent for most devices

## But what about input lag?

Jumpvalley tells Godot to poll user input faster than 60 times per second. This helps to alleviate input lag and make it more accurate as well.

See [Input.UseAccumulatedInput](https://docs.godotengine.org/en/4.1/classes/class_input.html#class-input-property-use-accumulated-input) for more details.

## References

[1]: [Godot Docs: Fixing jitter, stutter and input lag](https://docs.godotengine.org/en/stable/tutorials/rendering/jitter_stutter.html#jitter)
