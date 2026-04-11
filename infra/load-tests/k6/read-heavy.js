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
        read_heavy: {
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
    return seedData({ expenses: 24, incomes: 24 });
}

export default function (data) {
    const expenseId = pickRandom(data.expenseIds);
    const incomeId = pickRandom(data.incomeIds);

    group('read heavy', () => {
        ensureStatus(getJson('/api/expenses', 'GET /api/expenses'), 200, 'get expenses');
        ensureStatus(getJson('/api/incomes', 'GET /api/incomes'), 200, 'get incomes');
        ensureStatus(
            getJson(`/api/expenses/monthly?month=${data.month}&year=${data.year}`, 'GET /api/expenses/monthly'),
            200,
            'get monthly expenses');
        ensureStatus(
            getJson(`/api/incomes/monthly?month=${data.month}&year=${data.year}`, 'GET /api/incomes/monthly'),
            200,
            'get monthly incomes');
        ensureStatus(getJson(`/api/expenses/${expenseId}`, 'GET /api/expenses/:id'), 200, 'get expense by id');
        ensureStatus(getJson(`/api/incomes/${incomeId}`, 'GET /api/incomes/:id'), 200, 'get income by id');
    });

    shortSleep(0.1, 0.4);
}
