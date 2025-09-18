extends Button

const CLOSE_BUTTON_LOCALIZATION_KEY = "MENU_CLOSE"
const CLOSE_BUTTON_TOOLTIP_FONT = "res://addons/fonts/nunito/static/Nunito-Regular.ttf"

func _make_custom_tooltip(_for_text: String) -> Object:
	var custom_tooltip_text = tr(CLOSE_BUTTON_LOCALIZATION_KEY)
	if (shortcut != null && shortcut.events.size() > 0):
		var first_shortcut = shortcut.events[0]
		if (first_shortcut is InputEventKey):
			custom_tooltip_text += " ({0})".format([first_shortcut.as_text()])

	return TooltipHandler.make_tooltip_for_text(custom_tooltip_text)
