// API utility functions
const API_BASE = '/api';

const api = {
    // Budget Strategy API
    async calculateBudget(data) {
        try {
            const response = await axios.post(`${API_BASE}/budget/calculate`, data);
            return response.data;
        } catch (error) {
            console.error('Error calculating budget:', error);
            throw error;
        }
    },

    // Transaction Processing API
    async processTransactions(data) {
        try {
            const response = await axios.post(`${API_BASE}/transactions/process`, data);
            return response.data;
        } catch (error) {
            console.error('Error processing transactions:', error);
            throw error;
        }
    },

    // Mock Transaction Data API
    async getMockTransactions(accountId, fromDate, toDate) {
        try {
            const params = new URLSearchParams({
                fromDate: fromDate || new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
                toDate: toDate || new Date().toISOString().split('T')[0]
            });
            const response = await axios.get(`${API_BASE}/transactions/mock-data/${accountId}?${params}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching mock transactions:', error);
            throw error;
        }
    },

    // Report Generation API
    async generateSummaryReport(userId, startDate, endDate) {
        try {
            const params = new URLSearchParams();
            if (startDate) params.append('startDate', startDate);
            if (endDate) params.append('endDate', endDate);

            const response = await axios.get(`${API_BASE}/reports/summary/${userId}?${params}`);
            return response.data;
        } catch (error) {
            console.error('Error generating summary report:', error);
            throw error;
        }
    },

    async generateDetailedReport(userId, startDate, endDate) {
        try {
            const params = new URLSearchParams();
            if (startDate) params.append('startDate', startDate);
            if (endDate) params.append('endDate', endDate);

            const response = await axios.get(`${API_BASE}/reports/detailed/${userId}?${params}`);
            return response.data;
        } catch (error) {
            console.error('Error generating detailed report:', error);
            throw error;
        }
    }
};

// Utility functions
window.utils = {
    formatCurrency(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount || 0);
    },

    formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    },

    formatDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    generateId() {
        return Math.random().toString(36).substr(2, 9);
    },

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
};

window.api = api;