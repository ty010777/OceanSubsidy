import validate from "validate.js";

export const validateData = (data, rules, prefix = "") => {
    const constraints = {};

    Object.keys(rules).forEach((name) => {
        const rule = rules[name];

        if (validate.isString(rule)) {
            constraints[name] = { presence: { allowEmpty: false, message: rule.startsWith("^") ? rule : `^請輸入${rule}` } };
        } else {
            constraints[name] = rule;
        }
    });

    const errors = {};
    const result = validate(data, constraints);

    if (result) {
        Object.keys(result).forEach((name) => errors[`${prefix}${name}`] = result[name].join(","));
    }

    return errors;
};
