## spins a box around
class_name BoxSpinner

onready var _box: CSGBox
onready var _radiansPerSecond: float

func _init(box: CSGBox, radiansPerSecond: float):
	_box = box
	_radiansPerSecond = radiansPerSecond

func _process(_delta: float):
	pass

func rotate_in_frame(delta: float):
	if (_box != null && _radiansPerSecond != null):
		_box.rotate_y(_radiansPerSecond * delta)
