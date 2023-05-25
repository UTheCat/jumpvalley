extends Button


# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	var screen_resolution = get_viewport_rect().size
	self.set_size(screen_resolution)
