# Catppuccin Palette — Full Reference

Read this file before writing any color into output. Source: https://github.com/catppuccin/palette

## Mocha (default — dark)

| Token       | Hex        | RGB              | Role                          |
| ----------- | ---------- | ---------------- | ----------------------------- |
| rosewater   | `#f5e0dc`  | 245, 224, 220    | Accent (high-contrast alt)    |
| flamingo    | `#f2cdcd`  | 242, 205, 205    | Decorative                    |
| pink        | `#f5c2e7`  | 245, 194, 231    | Gradient end                  |
| mauve       | `#cba6f7`  | 203, 166, 247    | **Primary action**            |
| red         | `#f38ba8`  | 243, 139, 168    | **Error**                     |
| maroon      | `#eba0ac`  | 235, 160, 172    | Decorative                    |
| peach       | `#fab387`  | 250, 179, 135    | Decorative                    |
| yellow      | `#f9e2af`  | 249, 226, 175    | **Warning**                   |
| green       | `#a6e3a1`  | 166, 227, 161    | **Success**                   |
| teal        | `#94e2d5`  | 148, 226, 213    | Decorative                    |
| sky         | `#89dceb`  | 137, 220, 235    | **Info**                      |
| sapphire    | `#74c7ec`  | 116, 199, 236    | **Accent**                    |
| blue        | `#89b4fa`  | 137, 180, 250    | **Secondary**                 |
| lavender    | `#b4befe`  | 180, 190, 254    | High-contrast primary alt     |
| text        | `#cdd6f4`  | 205, 214, 244    | Headings                      |
| subtext1    | `#bac2de`  | 186, 194, 222    | Body                          |
| subtext0    | `#a6adc8`  | 166, 173, 200    | Secondary body                |
| overlay2    | `#9399b2`  | 147, 153, 178    | Heavy muted                   |
| overlay1    | `#7f849c`  | 127, 132, 156    | **Borders**                   |
| overlay0    | `#6c7086`  | 108, 112, 134    | Captions / muted              |
| surface2    | `#585b70`  | 88, 91, 112      | Disabled                      |
| surface1    | `#45475a`  | 69, 71, 90       | **Active / pressed**          |
| surface0    | `#313244`  | 49, 50, 68       | **Hover**                     |
| base        | `#1e1e2e`  | 30, 30, 46       | **Page background**           |
| mantle      | `#181825`  | 24, 24, 37       | **Sidebar / modal**           |
| crust       | `#11111b`  | 17, 17, 27       | **Footer / shadow base**      |

## Latte (light)

| Token       | Hex        | RGB              | Role                          |
| ----------- | ---------- | ---------------- | ----------------------------- |
| rosewater   | `#dc8a78`  | 220, 138, 120    | Accent (high-contrast alt)    |
| flamingo    | `#dd7878`  | 221, 120, 120    | Decorative                    |
| pink        | `#ea76cb`  | 234, 118, 203    | Gradient end                  |
| mauve       | `#8839ef`  | 136, 57, 239     | **Primary action**            |
| red         | `#d20f39`  | 210, 15, 57      | **Error**                     |
| maroon      | `#e64553`  | 230, 69, 83      | Decorative                    |
| peach       | `#fe640b`  | 254, 100, 11     | Decorative                    |
| yellow      | `#df8e1d`  | 223, 142, 29     | **Warning**                   |
| green       | `#40a02b`  | 64, 160, 43      | **Success**                   |
| teal        | `#179299`  | 23, 146, 153     | Decorative                    |
| sky         | `#04a5e5`  | 4, 165, 229      | **Info**                      |
| sapphire    | `#209fb5`  | 32, 159, 181     | **Accent**                    |
| blue        | `#1e66f5`  | 30, 102, 245     | **Secondary**                 |
| lavender    | `#7287fd`  | 114, 135, 253    | High-contrast primary alt     |
| text        | `#4c4f69`  | 76, 79, 105      | Headings                      |
| subtext1    | `#5c5f77`  | 92, 95, 119      | Body                          |
| subtext0    | `#6c6f85`  | 108, 111, 133    | Secondary body                |
| overlay2    | `#7c7f93`  | 124, 127, 147    | Heavy muted                   |
| overlay1    | `#8c8fa1`  | 140, 143, 161    | **Borders**                   |
| overlay0    | `#9ca0b0`  | 156, 160, 176    | Captions / muted              |
| surface2    | `#acb0be`  | 172, 176, 190    | Disabled                      |
| surface1    | `#bcc0cc`  | 188, 192, 204    | **Active / pressed**          |
| surface0    | `#ccd0da`  | 204, 208, 218    | **Hover**                     |
| base        | `#eff1f5`  | 239, 241, 245    | **Page background**           |
| mantle      | `#e6e9ef`  | 230, 233, 239    | **Sidebar / modal**           |
| crust       | `#dce0e8`  | 220, 224, 232    | **Footer / shadow base**      |

## Notes

- The `Frappé` and `Macchiato` flavors exist in the official palette but are intentionally **not** used by this skill to keep theme switching predictable. If a user explicitly asks for them, fetch the hex table from the upstream repo and add to this file before emitting.
- All values are official Catppuccin v1.x. Do not approximate or "round" hex codes for aesthetic reasons.
