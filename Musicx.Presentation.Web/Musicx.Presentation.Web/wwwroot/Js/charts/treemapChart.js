import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

export function renderTreemapChart(element, options) {
    if (!element || !options?.data) {
        clearChart(element);
        return;
    }

    const { data, width = Math.max(element.clientWidth || 800, 320), height = 500, padding = 1 } = options;
    clearChart(element);

    const root = d3
        .hierarchy(data)
        .sum(d => Number(d.value ?? 0))
        .sort((a, b) => (b.value ?? 0) - (a.value ?? 0));

    d3.treemap().size([width, height]).padding(padding).round(true)(root);

    const color = d3.scaleOrdinal(d3.schemeTableau10);

    const svg = d3
        .select(element)
        .append('svg')
        .attr('viewBox', [0, 0, width, height])
        .attr('width', width)
        .attr('height', height)
        .style('max-width', '100%');

    const leaf = svg
        .selectAll('g')
        .data(root.leaves())
        .join('g')
        .attr('transform', d => `translate(${d.x0},${d.y0})`);

    leaf
        .append('rect')
        .attr('fill', d => color(d.ancestors().at(-2)?.data?.name ?? d.data.name))
        .attr('fill-opacity', 0.8)
        .attr('width', d => Math.max(0, d.x1 - d.x0))
        .attr('height', d => Math.max(0, d.y1 - d.y0));

    leaf
        .append('title')
        .text(d => `${d.ancestors().map(node => node.data.name).reverse().join(' / ')}\n${d.value}`);

    leaf
        .append('text')
        .attr('x', 4)
        .attr('y', 14)
        .attr('fill', 'white')
        .attr('font-size', 11)
        .text(d => d.data.name);
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}