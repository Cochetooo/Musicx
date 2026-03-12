import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

export function renderBrushableScatterMatrix(element, options) {
    if (!element || !options?.data?.length || !options?.dimensions?.length) {
        clearChart(element);
        return;
    }

    const { data, dimensions, size = 130, padding = 24, pointRadius = 3 } = options;
    clearChart(element);

    const n = dimensions.length;
    const width = size * n + padding;
    const height = width;

    const x = dimensions.map(k => d3.scaleLinear().domain(d3.extent(data, d => +d[k])).range([padding / 2, size - padding / 2]));
    const y = x.map(scale => scale.copy().range([size - padding / 2, padding / 2]));

    const svg = d3
        .select(element)
        .append('svg')
        .attr('viewBox', [0, 0, width, height])
        .attr('width', width)
        .attr('height', height)
        .style('max-width', '100%');

    const cell = svg
        .append('g')
        .selectAll('g')
        .data(d3.cross(d3.range(n), d3.range(n)))
        .join('g')
        .attr('transform', ([i, j]) => `translate(${i * size},${j * size})`);

    cell.append('rect').attr('fill', 'none').attr('stroke', 'currentColor').attr('x', 0.5).attr('y', 0.5).attr('width', size - 1).attr('height', size - 1);

    const circles = cell
        .filter(([i, j]) => i !== j)
        .append('g')
        .selectAll('circle')
        .data(([i, j]) => data.map(d => ({ d, i, j })))
        .join('circle')
        .attr('cx', p => x[p.i](+p.d[dimensions[p.i]]))
        .attr('cy', p => y[p.j](+p.d[dimensions[p.j]]))
        .attr('r', pointRadius)
        .attr('fill', 'currentColor')
        .attr('fill-opacity', 0.45);

    cell.append('text').attr('x', 8).attr('y', 14).text(([i, j]) => (i === j ? dimensions[i] : ''));

    const brush = d3
        .brush()
        .extent([
            [0, 0],
            [size, size]
        ])
        .on('start brush end', brushed);

    cell.call(brush);

    function brushed(event, [i, j]) {
        const selection = event.selection;
        circles.attr('fill-opacity', p => {
            if (p.i !== i || p.j !== j || !selection) {
                return 0.45;
            }

            const [[x0, y0], [x1, y1]] = selection;
            const cx = x[p.i](+p.d[dimensions[p.i]]);
            const cy = y[p.j](+p.d[dimensions[p.j]]);

            return cx >= x0 && cx <= x1 && cy >= y0 && cy <= y1 ? 0.9 : 0.1;
        });
    }
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}