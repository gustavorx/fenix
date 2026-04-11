import { check, group } from 'k6';

import {
    ensureStatus,
    expensePayload,
    incomePayload,
    parseJson,
    postJson,
    shortSleep,
} from './lib/common.js';

export const options = {
    scenarios: {
        write_heavy: {
            executor: 'constant-vus',
            vus: 8,
            duration: '2m',
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<2000'],
    },
};

export default function () {
    group('write heavy', () => {
        if (Math.random() < 0.7) {
            const response = postJson(
                '/api/expenses',
                expensePayload(Math.floor(Math.random() * 1000), Math.random() < 0.5 ? 1 : 2),
                'POST /api/expenses');
            ensureStatus(response, 201, 'create expense');

            check(parseJson(response), {
                'write-heavy expense has id': body => typeof body.id === 'string' && body.id.length > 0,
            });

            return;
        }

        const response = postJson('/api/incomes', incomePayload(Math.floor(Math.random() * 1000)), 'POST /api/incomes');
        ensureStatus(response, 201, 'create income');

        check(parseJson(response), {
            'write-heavy income has id': body => typeof body.id === 'string' && body.id.length > 0,
        });
    });

    shortSleep(0.1, 0.3);
}
