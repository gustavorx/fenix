import { check, group } from 'k6';

import {
    ensureStatus,
    expensePayload,
    getJson,
    incomePayload,
    parseJson,
    pickRandom,
    postJson,
    seedData,
    shortSleep,
} from './lib/common.js';

export const options = {
    scenarios: {
        baseline: {
            executor: 'ramping-vus',
            startVUs: 1,
            stages: [
                { duration: '30s', target: 5 },
                { duration: '1m', target: 10 },
                { duration: '30s', target: 0 },
            ],
            gracefulRampDown: '10s',
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<1500'],
    },
};

export function setup() {
    return seedData();
}

export default function (data) {
    const operation = Math.random();

    if (operation < 0.55) {
        readOperation(data);
    } else {
        writeOperation();
    }

    shortSleep();
}

function readOperation(data) {
    const expenseId = pickRandom(data.expenseIds);
    const incomeId = pickRandom(data.incomeIds);

    group('baseline reads', () => {
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
}

function writeOperation() {
    group('baseline writes', () => {
        if (Math.random() < 0.6) {
            const response = postJson('/api/expenses', expensePayload(), 'POST /api/expenses');
            ensureStatus(response, 201, 'create expense');

            check(parseJson(response), {
                'created expense has id': body => typeof body.id === 'string' && body.id.length > 0,
            });

            return;
        }

        const response = postJson('/api/incomes', incomePayload(), 'POST /api/incomes');
        ensureStatus(response, 201, 'create income');

        check(parseJson(response), {
            'created income has id': body => typeof body.id === 'string' && body.id.length > 0,
        });
    });
}
