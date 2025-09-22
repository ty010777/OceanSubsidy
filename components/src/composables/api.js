const { ajax } = rxjs.ajax;
const { map } = rxjs;

let base = "";

const accessibility = (method, param = {}) => {
    return post("Service/OFS/Accessibility.ashx", { method, param });
};

const culture = (method, param = {}) => {
    return post("Service/OFS/Culture.ashx", { method, param });
};

const download = (path) => {
    return `${base}/Service/OFS/Download.ashx?path=${path}`;
};

const education = (method, param = {}) => {
    return post("Service/OFS/Education.ashx", { method, param });
};

const literacy = (method, param = {}) => {
    return post("Service/OFS/Literacy.ashx", { method, param });
};

const multiple = (method, param = {}) => {
    return post("Service/OFS/Multiple.ashx", { method, param });
};

const parse = ({ response, status }) => {
    if (status === 200) {
        if (response.success) {
            return response.data;
        } else {
            useMessageStore().error = response.message;
        }
    }
};

const post = (url, payload = {}) => {
    const headers = {};

    if (payload.constructor !== FormData) {
        headers["Content-Type"] = "application/json";
    }

    const options = { body: payload, headers, method: "POST", url: `${base}/${url}` };

    return ajax(options).pipe(map(parse));
};

const setBaseUrl = (url) => base = url;

const system = (method, param = {}) => {
    return post("Service/OFS/System.ashx", { method, param });
};

const toUrl = (relative) => `${base}${relative}`;

const upload = (file) => {
    const payload = new FormData();

    payload.append("file", file);

    return post("Service/OFS/Upload.ashx", payload);
};

export const api = Object.assign(post, { accessibility, culture, download, education, literacy, multiple, setBaseUrl, system, toUrl, upload });
