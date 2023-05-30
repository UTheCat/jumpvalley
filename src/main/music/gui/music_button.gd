extends Button

## The music button found in Juke's Towers of Hell
class_name MusicButton

var button_font = preload("res://fonts/ComicNeue-Angular-Bold.ttf")
var normal_stylebox = StyleBoxFlat.new()
var button_theme = Theme.new()

func _init():
	self.name = "MusicToggleButton"
	self.anchor_top = 1
	self.anchor_bottom = 1
	self.anchor_left = 1
	self.anchor_right = 1

	self.offset_left = -92
	self.offset_top = -37

	self.offset_bottom = -2
	self.offset_right = -2

	normal_stylebox.corner_radius_bottom_left = 3
	normal_stylebox.corner_radius_bottom_right = 3
	normal_stylebox.corner_radius_top_left = 3
	normal_stylebox.corner_radius_top_right = 3

	#normal_stylebox.bg_color = Color8(90, 142, 233, 255)
	self.add_theme_font_override("font", button_font)
	self.add_theme_font_size_override("font", 18)
	self.add_theme_stylebox_override("normal", normal_stylebox)
	#self.add_theme_constant_override("normal", button_theme)
	
	self.update(true)
	self.visible = true

## Updates the text to tell the user if the music is muted or not
##
## @param [code]isMusicOn[/code] Whether or not the music is playing (and therefore not muted)
func update(isMusicOn: bool):
	#self.remove_theme_color_override("bg_color")
	self.remove_theme_color_override("font_color")
	#self.remove_theme_stylebox_override("normal")

	if (isMusicOn):
		self.text = "Music: On"
		#self.add_theme_color_override("bg_color", Color(90, 142, 233))
		normal_stylebox.bg_color = Color8(90, 142, 233)
		self.add_theme_color_override("font_color", Color8(255, 255, 255))

		#button_style_box.bg_color = Color(90, 142, 233)
		#self.font_color = Color(255, 255, 255)
	else:
		self.text = "Music: Off"
		#self.add_theme_color_override("bg_color", Color(255, 255, 255))
		normal_stylebox.bg_color = Color8(255, 255, 255)
		self.add_theme_color_override("font_color", Color8(0, 0, 0))

		#button_style_box.bg_color = Color(255, 255, 255)
		#self.font_color = Color(0, 0, 0)
	
	#self.add_theme_stylebox_override("normal", normal_stylebox)


# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
