import { defineConfig } from "vite";
import AutoImport from "unplugin-auto-import/vite";
import vue from "@vitejs/plugin-vue";

export default defineConfig({
    build: {
        lib: {
            entry: "src/index.js",
            fileName: "ocean-subsidy-components",
            formats: ["umd"],
            name: "OceanSubsidyComponents"
        },
        rollupOptions: {
            external: [
                "pinia",
                "vue"
            ],
            output: {
                dir: "../OceanSubsidy/assets/js",
                globals: {
                    pinia: "Pinia",
                    vue: "Vue"
                }
            }
        },
        sourcemap: true
    },
    plugins: [
        AutoImport({
            dirs: [
                "./src/composables/**",
                "./src/stores/**",
            ],
            imports: [
                "pinia",
                "vue"
            ]
        }),
        vue({
            template: {
                transformAssetUrls: {
                    includeAbsolute: false
                }
            }
        })
    ]
});
