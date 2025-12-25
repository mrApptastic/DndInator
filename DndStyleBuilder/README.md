# DndInator Style Builder

Custom Bootstrap theme builder for DndInator application.

## Quick Start

```bash
# Install dependencies (first time only)
npm install

# Build and deploy to Blazor app
npm run deploy

# Or use watch mode for live development
npm run watch
```

## Setup

1. Install Node.js dependencies:
```bash
npm install
```

## Usage

### Build CSS (Production)
Generates minified CSS in `dist/dndinator-theme.css`:
```bash
npm run build
```

### Build CSS (Development)
Generates expanded CSS for easier debugging:
```bash
npm run build:expanded
```

### Watch Mode
Automatically rebuilds CSS when SCSS files change:
```bash
npm run watch
```

### Deploy to Blazor App
Build and automatically copy to DndInator/wwwroot/css/:
```bash
npm run deploy
```

For development version (expanded CSS):
```bash
npm run deploy:expanded
```

## Project Structure

```
DndStyleBuilder/
├── src/
│   ├── theme.scss          # Main entry point
│   ├── _variables.scss     # Custom Bootstrap variable overrides
│   └── _custom.scss        # Custom styles and extensions
├── dist/
│   └── dndinator-theme.css # Generated CSS output
├── package.json
└── README.md
```

## Customization

### Colors
Edit `src/_variables.scss` to customize colors:
- `$primary-dark` - Main brand color (dark blue)
- `$primary-purple` - Secondary brand color (purple)
- `$primary`, `$secondary`, etc. - Bootstrap semantic colors

### Components
Edit `src/_custom.scss` to add custom components or override Bootstrap styles:
- `.bg-gradient-primary` - Gradient background utility
- `.btn-gradient-primary` - Gradient button
- `.menu-button` - Custom menu button styling
- `.card-gradient-header` - Card with gradient header

### Typography & Spacing
Edit `src/_variables.scss` to adjust:
- Font families, sizes, and weights
- Spacing scale
- Border radius values
- Breakpoints

## Integration with DndInator

The theme is automatically deployed to your Blazor app with `npm run deploy`.

To use it, reference it in `wwwroot/index.html`:
```html
<link href="css/dndinator-theme.css" rel="stylesheet" />
```

## Development Workflow

1. Make changes to SCSS files in `src/`
2. Run `npm run watch` to auto-rebuild
3. Refresh browser to see changes
4. When satisfied, run `npm run build` for production CSS

## Bootstrap Version

This theme is built on Bootstrap 5.3.3. To update Bootstrap:
```bash
npm install bootstrap@latest
```
