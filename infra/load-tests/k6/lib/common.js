import http from 'k6/http';
import { check, fail, sleep } from 'k6';
import exec from 'k6/execution';

const baseUrl = (__ENV.BASE_URL || 'http://localhost:5207').replace(/\/$/, '');

export function seedData({ expenses = 12, incomes = 12 } = {}) {
    const monthYear = currentMonthYear();
    const expenseIds = [];
    const incomeIds = [];

    for (let index = 0; index < expenses; index += 1) {
        const paymentType = index % 2 === 0 ? 1 : 2;
        const response = postJson('/api/expenses', expensePayload(index, paymentType), 'POST /api/expenses [seed]');
        ensureStatus(response, 201, 'expense seed');

        const body = parseJson(response);
        expenseIds.push(body.id);
    }

    for (let index = 0; index < incomes; index += 1) {
        const response = postJson('/api/incomes', incomePayload(index), 'POST /api/incomes [seed]');
        ensureStatus(response, 201, 'income seed');

        const body = parseJson(response);
        incomeIds.push(body.id);
    }

    return {
        baseUrl,
        expenseIds,
        incomeIds,
        month: monthYear.month,
        year: monthYear.year,
    };
}

export function getJson(path, name) {
    return http.get(`${baseUrl}${path}`, params(name));
}

export function postJson(path, payload, name) {
    return http.post(`${baseUrl}${path}`, JSON.stringify(payload), params(name));
}

export function ensureStatus(response, expectedStatus, operation) {
    const ok = check(response, {
        [`${operation} returned ${expectedStatus}`]: item => item.status === expectedStatus,
    });

    if (!ok) {
        fail(`${operation} failed with status ${response.status}: ${response.body}`);
    }
}

export function pickRandom(items) {
    return items[Math.floor(Math.random() * items.length)];
}

export function shortSleep(minSeconds = 0.2, maxSeconds = 1.0) {
    sleep(randomBetween(minSeconds, maxSeconds));
}

export function expensePayload(sequence = 0, paymentType = 1) {
    const date = isoDateFromOffset(sequence % 7);
    const isInstallment = paymentType === 2;

    return {
        description: uniqueDescription(`k6-expense-${paymentType === 1 ? 'cash' : 'installment'}`),
        totalAmount: isInstallment ? 199.9 : 49.9,
        purchaseDate: date,
        paymentType,
        totalInstallments: isInstallment ? 3 : 1,
        firstDueDate: isInstallment ? date : null,
    };
}

export function incomePayload(sequence = 0) {
    return {
        description: uniqueDescription('k6-income'),
        amount: 150 + (sequence % 5) * 10,
        receivedDate: isoDateFromOffset(sequence % 7),
    };
}

export function currentMonthYear() {
    const now = new Date();
    return {
        month: now.getUTCMonth() + 1,
        year: now.getUTCFullYear(),
    };
}

export function parseJson(response) {
    try {
        return response.json();
    } catch (error) {
        fail(`Failed to parse JSON response: ${error} body=${response.body}`);
    }
}

function params(name) {
    return {
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'X-Correlation-ID': correlationId(name),
        },
        tags: { name },
    };
}

function correlationId(name) {
    return `${sanitizeName(name)}-${vuId()}-${iterationId()}-${Date.now()}`;
}

function uniqueDescription(prefix) {
    return `${prefix}-${vuId()}-${iterationId()}-${Math.floor(Math.random() * 1_000_000)}`;
}

function sanitizeName(name = 'request') {
    return name.toLowerCase().replace(/[^a-z0-9]+/g, '-');
}

function isoDateFromOffset(daysOffset) {
    const date = new Date();
    date.setUTCDate(date.getUTCDate() - daysOffset);
    return date.toISOString().slice(0, 10);
}

function randomBetween(min, max) {
    return min + Math.random() * (max - min);
}

function vuId() {
    try {
        return Number.isFinite(exec.vu.idInTest)
            ? exec.vu.idInTest
            : 0;
    } catch (_error) {
        return 0;
    }
}

function iterationId() {
    try {
        return Number.isFinite(exec.scenario.iterationInTest)
            ? exec.scenario.iterationInTest
            : 0;
    } catch (_error) {
        return 0;
    }
}
