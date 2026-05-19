/**
 * Catppuccin Tailwind extension.
 * Drop into your tailwind.config.js `theme.extend` block.
 * Class names: bg-ctp-base, text-ctp-subtext1, border-ctp-overlay1, etc.
 *
 * Theme switching via class strategy: add `dark` class for Mocha,
 * or use `data-theme="latte"` with the plugin pattern.
 */

module.exports = {
  darkMode: ['class'],
  theme: {
    extend: {
      colors: {
        'ctp': {
          // Mocha (default / dark)
          rosewater: '#f5e0dc',
          flamingo:  '#f2cdcd',
          pink:      '#f5c2e7',
          mauve:     '#cba6f7',
          red:       '#f38ba8',
          maroon:    '#eba0ac',
          peach:     '#fab387',
          yellow:    '#f9e2af',
          green:     '#a6e3a1',
          teal:      '#94e2d5',
          sky:       '#89dceb',
          sapphire:  '#74c7ec',
          blue:      '#89b4fa',
          lavender:  '#b4befe',
          text:      '#cdd6f4',
          subtext1:  '#bac2de',
          subtext0:  '#a6adc8',
          overlay2:  '#9399b2',
          overlay1:  '#7f849c',
          overlay0:  '#6c7086',
          surface2:  '#585b70',
          surface1:  '#45475a',
          surface0:  '#313244',
          base:      '#1e1e2e',
          mantle:    '#181825',
          crust:     '#11111b',
        },
        'ctp-latte': {
          rosewater: '#dc8a78',
          flamingo:  '#dd7878',
          pink:      '#ea76cb',
          mauve:     '#8839ef',
          red:       '#d20f39',
          maroon:    '#e64553',
          peach:     '#fe640b',
          yellow:    '#df8e1d',
          green:     '#40a02b',
          teal:      '#179299',
          sky:       '#04a5e5',
          sapphire:  '#209fb5',
          blue:      '#1e66f5',
          lavender:  '#7287fd',
          text:      '#4c4f69',
          subtext1:  '#5c5f77',
          subtext0:  '#6c6f85',
          overlay2:  '#7c7f93',
          overlay1:  '#8c8fa1',
          overlay0:  '#9ca0b0',
          surface2:  '#acb0be',
          surface1:  '#bcc0cc',
          surface0:  '#ccd0da',
          base:      '#eff1f5',
          mantle:    '#e6e9ef',
          crust:     '#dce0e8',
        },
      },
      borderRadius: {
        DEFAULT: '8px',
        'ctp-sm':  '4px',
        'ctp-md':  '8px',
        'ctp-lg':  '12px',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['JetBrains Mono', 'Fira Code', 'monospace'],
      },
      boxShadow: {
        'ctp-sm': '0 2px 4px rgba(17, 17, 27, 0.3)',
        'ctp-md': '0 4px 12px rgba(17, 17, 27, 0.4)',
        'ctp-lg': '0 8px 24px rgba(17, 17, 27, 0.5)',
      },
      backgroundImage: {
        'ctp-gradient': 'linear-gradient(135deg, #cba6f7 0%, #f5c2e7 100%)',
      },
    },
  },
  plugins: [],
};
