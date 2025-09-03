let current = Date.now();

export const isProjectEditable = (type, status, step) => {
    const store = useProgressStore();

    if ([1, 3].includes(store[type].status)) { //申請中,退回補正
        return true;
    }

    if ([1, 4].includes(store[type].changeStatus)) { //變更申請: 編輯中,退回修改
        return true;
    }

    if (store[type].status === 10) { //修正計畫書
        switch (type) {
            case "culture":
                if ([2, 3, 5].includes(step)) {
                    return true;
                }
                break;
            case "multiple":
                if ([2, 3, 4, 5].includes(step)) {
                    return true;
                }
                break;
            case "accessibility":
                if ([2, 3, 4, 5].includes(step)) {
                    return true;
                }
                break;
        }
    }

    return false;
};

export const nextId = () => `id--${current++}`;

export const toInt = (value) => parseInt(value) || 0;
