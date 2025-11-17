// Chart utility functions using Chart.js
window.charts = {
    spendingChart: null,
    categoryChart: null,

    initializeCharts() {
        this.initSpendingChart();
        this.initCategoryChart();
    },

    initSpendingChart() {
        const ctx = document.getElementById('spendingChart');
        if (!ctx) return;

        this.spendingChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: this.generateDateLabels(30),
                datasets: [{
                    label: 'Daily Spending',
                    data: this.generateSpendingData(30),
                    borderColor: 'rgb(59, 130, 246)',
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '$' + value;
                            }
                        }
                    }
                },
                elements: {
                    point: {
                        radius: 4,
                        hoverRadius: 6
                    }
                }
            }
        });
    },

    initCategoryChart() {
        const ctx = document.getElementById('categoryChart');
        if (!ctx) return;

        this.categoryChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Groceries', 'Entertainment', 'Transportation', 'Utilities', 'Healthcare'],
                datasets: [{
                    data: [400, 200, 150, 100, 50],
                    backgroundColor: [
                        'rgb(34, 197, 94)',   // Green
                        'rgb(59, 130, 246)',  // Blue
                        'rgb(245, 158, 11)',  // Yellow
                        'rgb(239, 68, 68)',   // Red
                        'rgb(139, 92, 246)'   // Purple
                    ],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            padding: 15
                        }
                    }
                },
                cutout: '60%'
            }
        });
    },

    updateSpendingChart(data) {
        if (!this.spendingChart) return;

        this.spendingChart.data.datasets[0].data = data;
        this.spendingChart.update();
    },

    updateCategoryChart(categories, amounts) {
        if (!this.categoryChart) return;

        this.categoryChart.data.labels = categories;
        this.categoryChart.data.datasets[0].data = amounts;
        this.categoryChart.update();
    },

    generateDateLabels(days) {
        const labels = [];
        for (let i = days - 1; i >= 0; i--) {
            const date = new Date();
            date.setDate(date.getDate() - i);
            labels.push(date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
        }
        return labels;
    },

    generateSpendingData(days) {
        const data = [];
        for (let i = 0; i < days; i++) {
            // Generate realistic spending data with some randomness
            const baseAmount = 25 + Math.random() * 75;
            const weekendMultiplier = (i % 7 === 0 || i % 7 === 6) ? 1.5 : 1;
            data.push(Math.round(baseAmount * weekendMultiplier));
        }
        return data;
    },

    destroyCharts() {
        if (this.spendingChart) {
            this.spendingChart.destroy();
            this.spendingChart = null;
        }
        if (this.categoryChart) {
            this.categoryChart.destroy();
            this.categoryChart = null;
        }
    }
};

// Initialize charts when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        window.charts.initializeCharts();
    }, 100);
});