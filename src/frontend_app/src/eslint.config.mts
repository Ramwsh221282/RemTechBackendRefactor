import typescriptEslint from "@typescript-eslint/eslint-plugin";
import typescriptParser from "@typescript-eslint/parser";
import angularEslint from "@angular-eslint/eslint-plugin";
import { dirname } from "node:path";
import { fileURLToPath } from "node:url";
const __dirname = dirname(fileURLToPath(import.meta.url));

export default [
	{
		files: ["**/*.ts"],
		languageOptions: {
			parser: typescriptParser,
			parserOptions: {
				project: "./tsconfig.json",
				tsconfigRootDir: __dirname,
				sourceType: "module",
			},
		},
		plugins: {
			"@typescript-eslint": typescriptEslint,
			"@angular-eslint": angularEslint,
		},
		rules: {
			"@typescript-eslint/typedef": [
				"error",
				{
					arrayDestructuring: true,
					arrowParameter: true,
					memberVariableDeclaration: true,
					objectDestructuring: true,
					parameter: true,
					propertyDeclaration: true,
					variableDeclaration: true,
				},
			],
			"@typescript-eslint/explicit-function-return-type": "error",
			"@typescript-eslint/explicit-module-boundary-types": "error",
			"@typescript-eslint/no-inferrable-types": "error",
			"@typescript-eslint/no-explicit-any": "error",
			"@typescript-eslint/no-unsafe-assignment": "error",
			"@typescript-eslint/no-unsafe-member-access": "error",
			"@typescript-eslint/no-unsafe-call": "error",
			"@angular-eslint/directive-selector": ["error", { type: "attribute", prefix: "app", style: "camelCase" }],
			"@angular-eslint/component-selector": ["error", { type: "element", prefix: "app", style: "kebab-case" }],
		},
	},
];
