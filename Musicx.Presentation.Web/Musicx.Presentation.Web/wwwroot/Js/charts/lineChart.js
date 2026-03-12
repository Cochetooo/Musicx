import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

/**
 * @typedef {{x: string, y: number}} LinePoint
 */

export function renderLineChart(element, options) {
    if (!element || !options?.points?.length) {
        clearChart(element);
        return;
    }

    const {
        points,
        height = 220,
        marginTop = 12,
        marginRight = 16,
        marginBottom = 30,
        marginLeft = 42,
        strokeColor = '#7B61FF',
        fillColor = 'rgba(123, 97, 255, 0.22)',
        gridColor = 'rgba(255, 255, 255, 0.12)',
        tickColor = 'rgba(255, 255, 255, 0.72)'
    } = options;

    const width = Math.max(element.clientWidth || 300, 300);
    clearChart(element);

    const svg = d3
        .select(element)
        .append('svg')
        .attr('width', width)
        .attr('height', height)
        .attr('viewBox', `0 0 ${width} ${height}`)
        .attr('preserveAspectRatio', 'none');

    const xScale = d3
        .scalePoint()
        .domain(points.map(point => point.x))
        .range([marginLeft, width - marginRight])
        .padding(0.4);

    const yMax = d3.max(points, point => point.y) ?? 0;
    const yScale = d3
        .scaleLinear()
        .domain([0, Math.max(1, yMax * 1.1)])
        .range([height - marginBottom, marginTop])
        .nice(5);

    const areaGenerator = d3
        .area()
        .x(point => xScale(point.x))
        .y0(height - marginBottom)
        .y1(point => yScale(point.y))
        .curve(d3.curveCatmullRom.alpha(0.5));

    const lineGenerator = d3
        .line()
        .x(point => xScale(point.x))
        .y(point => yScale(point.y))
        .curve(d3.curveCatmullRom.alpha(0.5));

    svg
        .append('g')
        .attr('transform', `translate(0,${height - marginBottom})`)
        .call(d3.axisBottom(xScale).tickSize(0))
        .call(group => group.select('.domain').remove())
        .call(group => group.selectAll('text').attr('fill', tickColor).style('font-size', '11px'));

    svg
        .append('g')
        .attr('transform', `translate(${marginLeft},0)`)
        .call(d3.axisLeft(yScale).ticks(5).tickSize(-(width - marginLeft - marginRight)))
        .call(group => group.select('.domain').remove())
        .call(group => group.selectAll('.tick line').attr('stroke', gridColor))
        .call(group => group.selectAll('.tick text').attr('fill', tickColor).style('font-size', '11px'));

    svg.append('path').datum(points).attr('fill', fillColor).attr('d', areaGenerator);

    svg
        .append('path')
        .datum(points)
        .attr('fill', 'none')
        .attr('stroke', strokeColor)
        .attr('stroke-width', 3)
        .attr('stroke-linecap', 'round')
        .attr('stroke-linejoin', 'round')
        .attr('d', lineGenerator);

    svg
        .append('g')
        .selectAll('circle')
        .data(points)
        .join('circle')
        .attr('cx', point => xScale(point.x))
        .attr('cy', point => yScale(point.y))
        .attr('r', 3)
        .attr('fill', strokeColor);
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}