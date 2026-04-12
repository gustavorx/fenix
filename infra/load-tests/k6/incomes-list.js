import { group } from 'k6';

import {
    ensureStatus,
    getJson,
    seedData,
    shortSleep,
} from './lib/common.js';

export const options = {
    scenarios: {
        incomes_list: {
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

export default function () {
    group('incomes list', () => {
        ensureStatus(getJson('/api/incomes', 'GET /api/incomes'), 200, 'get incomes');
    });

    shortSleep(0.1, 0.4);
}
