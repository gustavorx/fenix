import { group } from 'k6';

import {
    ensureStatus,
    getJson,
    pickRandom,
    seedData,
    shortSleep,
} from './lib/common.js';

export const options = {
    scenarios: {
        expenses_by_id: {
            executor: 'constant-vus',
            vus: 20,
            duration: '2m',
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.02'],
        http_req_duration: ['p(95)<1000'],
    },
};

export function setup() {
    return seedData({ expenses: 24, incomes: 0 });
}

export default function (data) {
    const expenseId = pickRandom(data.expenseIds);

    group('expenses by id', () => {
        ensureStatus(getJson(`/api/expenses/${expenseId}`, 'GET /api/expenses/:id'), 200, 'get expense by id');
    });

    shortSleep(0.1, 0.4);
}
