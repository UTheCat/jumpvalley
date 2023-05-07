extends Spatial


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

var main_gui = Control.new()
var fps_counter = Label.new()


# Called when the node enters the scene tree for the first time.
func _ready():
	fps_counter.name = "FPSCounter"
	fps_counter.text = "FPS: "

	main_gui.add_child(fps_counter)
	self.add_child(main_gui)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fps_counter.text = "FPS: " + str(1 / delta)
