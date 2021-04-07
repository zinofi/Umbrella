const autoprefixer = require('autoprefixer');
const pseudoelements = require('postcss-pseudoelements');

module.exports = {
  plugins: [
    autoprefixer({
      overrideBrowserslist: ['>1%', 'last 2 versions', 'not dead']
    }),
    pseudoelements()
  ]
};