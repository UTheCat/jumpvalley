extends Node
class_name TooltipHandler

const TOOLTIP_FONT_PATH = "res://addons/fonts/nunito/static/Nunito-Regular.ttf"

static var tooltip_font = load(TOOLTIP_FONT_PATH)

## Makes a tooltip label with Jumpvalley's primary font ("Nunito" by Vernon Adams, Cyreal, and Jacques Le Bailly)
static func make_tooltip_for_text(for_text) -> Label:
	var label = Label.new()
	label.text = for_text
	label.add_theme_font_override("font", tooltip_font)
	return label
