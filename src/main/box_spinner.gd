## spins a box around
class_name BoxSpinner

var _box: CSGBox3D
var _radiansPerSecond: float

func _init(box: CSGBox3D, radiansPerSecond: float):
	_box = box
	_radiansPerSecond = radiansPerSecond

func _process(_delta: float):
	pass

func rotate_in_frame(delta: float):
	if (_box != null && _radiansPerSecond != null):
		_box.rotate_y(_radiansPerSecond * delta)
