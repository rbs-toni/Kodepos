import * as esbuild from 'esbuild'

await esbuild.build({
    entryPoints: ['./Scripts/floating-ui.js'],
    minify: true,
    bundle: true,
    sourcemap: false,
    logLevel: 'info',
    target: 'es2022',
    format: 'esm',
    outfile: './wwwroot/lib/floating-ui/floating-ui.min.js',
    legalComments: 'none'
})