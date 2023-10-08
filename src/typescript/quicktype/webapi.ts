// To parse this data:
//
//   import { Convert, Webapi } from "./file";
//
//   const webapi = Convert.toWebapi(json);
//
// These functions will throw an error if the JSON doesn't
// match the expected interface, even if the JSON is valid.

export interface Webapi {
    openapi:    string;
    info:       Info;
    paths:      Paths;
    components: Components;
}

export interface Components {
    schemas: Schemas;
}

export interface Schemas {
    SimpleInput:     SimpleInput;
    SimpleOutput:    SimpleOutput;
    WeatherForecast: WeatherForecast;
}

export interface SimpleInput {
    type:                 string;
    properties:           SimpleInputProperties;
    additionalProperties: boolean;
}

export interface SimpleInputProperties {
    input: Input;
}

export interface Input {
    type:     string;
    nullable: boolean;
}

export interface SimpleOutput {
    type:                 string;
    properties:           SimpleOutputProperties;
    additionalProperties: boolean;
}

export interface SimpleOutputProperties {
    output: Input;
}

export interface WeatherForecast {
    type:                 string;
    properties:           WeatherForecastProperties;
    additionalProperties: boolean;
}

export interface WeatherForecastProperties {
    date:         DateClass;
    temperatureC: DateClass;
    temperatureF: TemperatureF;
    summary:      Input;
}

export interface DateClass {
    type:   string;
    format: string;
}

export interface TemperatureF {
    type:     string;
    format:   string;
    readOnly: boolean;
}

export interface Info {
    title:   string;
    version: string;
}

export interface Paths {
    "/Simple":          Simple;
    "/WeatherForecast": WeatherForecastClass;
}

export interface Simple {
    get:  PostClass;
    post: PostClass;
}

export interface PostClass {
    tags:         string[];
    operationId?: string;
    requestBody:  RequestBody;
    responses:    PostResponses;
}

export interface RequestBody {
    content: RequestBodyContent;
}

export interface RequestBodyContent {
    "application/json":   ApplicationJSONClass;
    "text/json":          ApplicationJSONClass;
    "application/*+json": ApplicationJSONClass;
}

export interface ApplicationJSONClass {
    schema: ItemsClass;
}

export interface ItemsClass {
    $ref: Ref;
}

export enum Ref {
    ComponentsSchemasSimpleInput = "#/components/schemas/SimpleInput",
    ComponentsSchemasSimpleOutput = "#/components/schemas/SimpleOutput",
    ComponentsSchemasWeatherForecast = "#/components/schemas/WeatherForecast",
}

export interface PostResponses {
    "200": Purple200;
}

export interface Purple200 {
    description: string;
    content:     PurpleContent;
}

export interface PurpleContent {
    "text/plain":       ApplicationJSONClass;
    "application/json": ApplicationJSONClass;
    "text/json":        ApplicationJSONClass;
}

export interface WeatherForecastClass {
    get:  PurpleGet;
    post: PostClass;
}

export interface PurpleGet {
    tags:        string[];
    operationId: string;
    responses:   PurpleResponses;
}

export interface PurpleResponses {
    "200": Fluffy200;
}

export interface Fluffy200 {
    description: string;
    content:     FluffyContent;
}

export interface FluffyContent {
    "text/plain":       PurpleApplicationJSON;
    "application/json": PurpleApplicationJSON;
    "text/json":        PurpleApplicationJSON;
}

export interface PurpleApplicationJSON {
    schema: PurpleSchema;
}

export interface PurpleSchema {
    type:  string;
    items: ItemsClass;
}

// Converts JSON strings to/from your types
// and asserts the results of JSON.parse at runtime
export class Convert {
    public static toWebapi(json: string): Webapi {
        return cast(JSON.parse(json), r("Webapi"));
    }

    public static webapiToJson(value: Webapi): string {
        return JSON.stringify(uncast(value, r("Webapi")), null, 2);
    }
}

function invalidValue(typ: any, val: any, key: any, parent: any = ''): never {
    const prettyTyp = prettyTypeName(typ);
    const parentText = parent ? ` on ${parent}` : '';
    const keyText = key ? ` for key "${key}"` : '';
    throw Error(`Invalid value${keyText}${parentText}. Expected ${prettyTyp} but got ${JSON.stringify(val)}`);
}

function prettyTypeName(typ: any): string {
    if (Array.isArray(typ)) {
        if (typ.length === 2 && typ[0] === undefined) {
            return `an optional ${prettyTypeName(typ[1])}`;
        } else {
            return `one of [${typ.map(a => { return prettyTypeName(a); }).join(", ")}]`;
        }
    } else if (typeof typ === "object" && typ.literal !== undefined) {
        return typ.literal;
    } else {
        return typeof typ;
    }
}

function jsonToJSProps(typ: any): any {
    if (typ.jsonToJS === undefined) {
        const map: any = {};
        typ.props.forEach((p: any) => map[p.json] = { key: p.js, typ: p.typ });
        typ.jsonToJS = map;
    }
    return typ.jsonToJS;
}

function jsToJSONProps(typ: any): any {
    if (typ.jsToJSON === undefined) {
        const map: any = {};
        typ.props.forEach((p: any) => map[p.js] = { key: p.json, typ: p.typ });
        typ.jsToJSON = map;
    }
    return typ.jsToJSON;
}

function transform(val: any, typ: any, getProps: any, key: any = '', parent: any = ''): any {
    function transformPrimitive(typ: string, val: any): any {
        if (typeof typ === typeof val) return val;
        return invalidValue(typ, val, key, parent);
    }

    function transformUnion(typs: any[], val: any): any {
        // val must validate against one typ in typs
        const l = typs.length;
        for (let i = 0; i < l; i++) {
            const typ = typs[i];
            try {
                return transform(val, typ, getProps);
            } catch (_) {}
        }
        return invalidValue(typs, val, key, parent);
    }

    function transformEnum(cases: string[], val: any): any {
        if (cases.indexOf(val) !== -1) return val;
        return invalidValue(cases.map(a => { return l(a); }), val, key, parent);
    }

    function transformArray(typ: any, val: any): any {
        // val must be an array with no invalid elements
        if (!Array.isArray(val)) return invalidValue(l("array"), val, key, parent);
        return val.map(el => transform(el, typ, getProps));
    }

    function transformDate(val: any): any {
        if (val === null) {
            return null;
        }
        const d = new Date(val);
        if (isNaN(d.valueOf())) {
            return invalidValue(l("Date"), val, key, parent);
        }
        return d;
    }

    function transformObject(props: { [k: string]: any }, additional: any, val: any): any {
        if (val === null || typeof val !== "object" || Array.isArray(val)) {
            return invalidValue(l(ref || "object"), val, key, parent);
        }
        const result: any = {};
        Object.getOwnPropertyNames(props).forEach(key => {
            const prop = props[key];
            const v = Object.prototype.hasOwnProperty.call(val, key) ? val[key] : undefined;
            result[prop.key] = transform(v, prop.typ, getProps, key, ref);
        });
        Object.getOwnPropertyNames(val).forEach(key => {
            if (!Object.prototype.hasOwnProperty.call(props, key)) {
                result[key] = transform(val[key], additional, getProps, key, ref);
            }
        });
        return result;
    }

    if (typ === "any") return val;
    if (typ === null) {
        if (val === null) return val;
        return invalidValue(typ, val, key, parent);
    }
    if (typ === false) return invalidValue(typ, val, key, parent);
    let ref: any = undefined;
    while (typeof typ === "object" && typ.ref !== undefined) {
        ref = typ.ref;
        typ = typeMap[typ.ref];
    }
    if (Array.isArray(typ)) return transformEnum(typ, val);
    if (typeof typ === "object") {
        return typ.hasOwnProperty("unionMembers") ? transformUnion(typ.unionMembers, val)
            : typ.hasOwnProperty("arrayItems")    ? transformArray(typ.arrayItems, val)
            : typ.hasOwnProperty("props")         ? transformObject(getProps(typ), typ.additional, val)
            : invalidValue(typ, val, key, parent);
    }
    // Numbers can be parsed by Date but shouldn't be.
    if (typ === Date && typeof val !== "number") return transformDate(val);
    return transformPrimitive(typ, val);
}

function cast<T>(val: any, typ: any): T {
    return transform(val, typ, jsonToJSProps);
}

function uncast<T>(val: T, typ: any): any {
    return transform(val, typ, jsToJSONProps);
}

function l(typ: any) {
    return { literal: typ };
}

function a(typ: any) {
    return { arrayItems: typ };
}

function u(...typs: any[]) {
    return { unionMembers: typs };
}

function o(props: any[], additional: any) {
    return { props, additional };
}

function m(additional: any) {
    return { props: [], additional };
}

function r(name: string) {
    return { ref: name };
}

const typeMap: any = {
    "Webapi": o([
        { json: "openapi", js: "openapi", typ: "" },
        { json: "info", js: "info", typ: r("Info") },
        { json: "paths", js: "paths", typ: r("Paths") },
        { json: "components", js: "components", typ: r("Components") },
    ], false),
    "Components": o([
        { json: "schemas", js: "schemas", typ: r("Schemas") },
    ], false),
    "Schemas": o([
        { json: "SimpleInput", js: "SimpleInput", typ: r("SimpleInput") },
        { json: "SimpleOutput", js: "SimpleOutput", typ: r("SimpleOutput") },
        { json: "WeatherForecast", js: "WeatherForecast", typ: r("WeatherForecast") },
    ], false),
    "SimpleInput": o([
        { json: "type", js: "type", typ: "" },
        { json: "properties", js: "properties", typ: r("SimpleInputProperties") },
        { json: "additionalProperties", js: "additionalProperties", typ: true },
    ], false),
    "SimpleInputProperties": o([
        { json: "input", js: "input", typ: r("Input") },
    ], false),
    "Input": o([
        { json: "type", js: "type", typ: "" },
        { json: "nullable", js: "nullable", typ: true },
    ], false),
    "SimpleOutput": o([
        { json: "type", js: "type", typ: "" },
        { json: "properties", js: "properties", typ: r("SimpleOutputProperties") },
        { json: "additionalProperties", js: "additionalProperties", typ: true },
    ], false),
    "SimpleOutputProperties": o([
        { json: "output", js: "output", typ: r("Input") },
    ], false),
    "WeatherForecast": o([
        { json: "type", js: "type", typ: "" },
        { json: "properties", js: "properties", typ: r("WeatherForecastProperties") },
        { json: "additionalProperties", js: "additionalProperties", typ: true },
    ], false),
    "WeatherForecastProperties": o([
        { json: "date", js: "date", typ: r("DateClass") },
        { json: "temperatureC", js: "temperatureC", typ: r("DateClass") },
        { json: "temperatureF", js: "temperatureF", typ: r("TemperatureF") },
        { json: "summary", js: "summary", typ: r("Input") },
    ], false),
    "DateClass": o([
        { json: "type", js: "type", typ: "" },
        { json: "format", js: "format", typ: "" },
    ], false),
    "TemperatureF": o([
        { json: "type", js: "type", typ: "" },
        { json: "format", js: "format", typ: "" },
        { json: "readOnly", js: "readOnly", typ: true },
    ], false),
    "Info": o([
        { json: "title", js: "title", typ: "" },
        { json: "version", js: "version", typ: "" },
    ], false),
    "Paths": o([
        { json: "/Simple", js: "/Simple", typ: r("Simple") },
        { json: "/WeatherForecast", js: "/WeatherForecast", typ: r("WeatherForecastClass") },
    ], false),
    "Simple": o([
        { json: "get", js: "get", typ: r("PostClass") },
        { json: "post", js: "post", typ: r("PostClass") },
    ], false),
    "PostClass": o([
        { json: "tags", js: "tags", typ: a("") },
        { json: "operationId", js: "operationId", typ: u(undefined, "") },
        { json: "requestBody", js: "requestBody", typ: r("RequestBody") },
        { json: "responses", js: "responses", typ: r("PostResponses") },
    ], false),
    "RequestBody": o([
        { json: "content", js: "content", typ: r("RequestBodyContent") },
    ], false),
    "RequestBodyContent": o([
        { json: "application/json", js: "application/json", typ: r("ApplicationJSONClass") },
        { json: "text/json", js: "text/json", typ: r("ApplicationJSONClass") },
        { json: "application/*+json", js: "application/*+json", typ: r("ApplicationJSONClass") },
    ], false),
    "ApplicationJSONClass": o([
        { json: "schema", js: "schema", typ: r("ItemsClass") },
    ], false),
    "ItemsClass": o([
        { json: "$ref", js: "$ref", typ: r("Ref") },
    ], false),
    "PostResponses": o([
        { json: "200", js: "200", typ: r("Purple200") },
    ], false),
    "Purple200": o([
        { json: "description", js: "description", typ: "" },
        { json: "content", js: "content", typ: r("PurpleContent") },
    ], false),
    "PurpleContent": o([
        { json: "text/plain", js: "text/plain", typ: r("ApplicationJSONClass") },
        { json: "application/json", js: "application/json", typ: r("ApplicationJSONClass") },
        { json: "text/json", js: "text/json", typ: r("ApplicationJSONClass") },
    ], false),
    "WeatherForecastClass": o([
        { json: "get", js: "get", typ: r("PurpleGet") },
        { json: "post", js: "post", typ: r("PostClass") },
    ], false),
    "PurpleGet": o([
        { json: "tags", js: "tags", typ: a("") },
        { json: "operationId", js: "operationId", typ: "" },
        { json: "responses", js: "responses", typ: r("PurpleResponses") },
    ], false),
    "PurpleResponses": o([
        { json: "200", js: "200", typ: r("Fluffy200") },
    ], false),
    "Fluffy200": o([
        { json: "description", js: "description", typ: "" },
        { json: "content", js: "content", typ: r("FluffyContent") },
    ], false),
    "FluffyContent": o([
        { json: "text/plain", js: "text/plain", typ: r("PurpleApplicationJSON") },
        { json: "application/json", js: "application/json", typ: r("PurpleApplicationJSON") },
        { json: "text/json", js: "text/json", typ: r("PurpleApplicationJSON") },
    ], false),
    "PurpleApplicationJSON": o([
        { json: "schema", js: "schema", typ: r("PurpleSchema") },
    ], false),
    "PurpleSchema": o([
        { json: "type", js: "type", typ: "" },
        { json: "items", js: "items", typ: r("ItemsClass") },
    ], false),
    "Ref": [
        "#/components/schemas/SimpleInput",
        "#/components/schemas/SimpleOutput",
        "#/components/schemas/WeatherForecast",
    ],
};
