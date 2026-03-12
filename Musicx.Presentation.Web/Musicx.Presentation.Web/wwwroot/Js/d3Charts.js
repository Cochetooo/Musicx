export { renderLineChart, clearChart as clearLineChart } from './charts/lineChart.js';
export { renderDivergingBarChart, clearChart as clearDivergingBarChart } from './charts/divergingBarChart.js';
export { renderBrushableScatterMatrix } from './charts/brushableScatterMatrix.js';
export { renderTreemapChart } from './charts/treemapChart.js';
export { renderGenreArchitectureChart } from './charts/genreArchitectureChart.js';

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}