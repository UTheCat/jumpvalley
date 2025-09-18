extends Button

func _make_custom_tooltip(_for_text: String) -> Object:
	var custom_tooltip_text = ""
	var hotkey = shortcut
	if (hotkey != null):
		hotkey = hotkey.events[0]
		if (hotkey != null && hotkey is InputEventKey && hotkey.keycode != 0):
			custom_tooltip_text = "(" + hotkey.as_text_keycode() + ")"
	
	return TooltipHandler.make_tooltip_for_text(custom_tooltip_text)
