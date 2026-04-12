import { group } from 'k6';

import {
    ensureStatus,
    getJson,
    seedData,
    shortSleep,
} from './lib/common.js';

export const options = {
    scenarios: {
        incomes_monthly: {
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
    return seedData({ expenses: 0, incomes: 24 });
}

export default function (data) {
    group('incomes monthly', () => {
        ensureStatus(
            getJson(`/api/incomes/monthly?month=${data.month}&year=${data.year}`, 'GET /api/incomes/monthly'),
            200,
            'get monthly incomes');
    });

    shortSleep(0.1, 0.4);
}
