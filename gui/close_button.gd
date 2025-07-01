extends Button

const CLOSE_BUTTON_TOOLTIP = "Close %s" # %s is replaced with the keybind which activates the close button
const CLOSE_BUTTON_TOOLTIP_FONT = "res://addons/fonts/nunito/static/Nunito-Regular.ttf"

var font = load(CLOSE_BUTTON_TOOLTIP_FONT)

func _make_custom_tooltip(_for_text: String) -> Object:
    var shortcut_text = ""
    if (shortcut != null && shortcut.events.size() > 0):
        var first_shortcut = shortcut.events[0]
        if (first_shortcut is InputEventKey):
            shortcut_text = first_shortcut.as_text()

    var label = Label.new()
    label.text = CLOSE_BUTTON_TOOLTIP.format(shortcut_text)
    label.add_theme_font_override("font", font)
    return label
