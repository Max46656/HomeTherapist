const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'http://localhost:5000',
      changeOrigin: true,
      pathRewrite: {
        '^/api': ''
      },
      bypass: function(req, res, proxyOptions) {
        if (req.originalUrl === '/favicon.ico') {
          return '/favicon.ico';
        }
      }
    })
  );
};
