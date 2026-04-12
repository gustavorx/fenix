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
        incomes_by_id: {
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
    const incomeId = pickRandom(data.incomeIds);

    group('incomes by id', () => {
        ensureStatus(getJson(`/api/incomes/${incomeId}`, 'GET /api/incomes/:id'), 200, 'get income by id');
    });

    shortSleep(0.1, 0.4);
}
