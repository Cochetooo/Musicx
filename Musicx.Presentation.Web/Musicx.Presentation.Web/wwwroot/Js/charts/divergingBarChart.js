import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

/**
 * @typedef {{label: string, value: number}} DivergingBarDatum
 */

const LABEL_KEYS = ['label', 'Label', 'state', 'State', 'name', 'Name'];

function extractLabel(item, index) {
    for (const key of LABEL_KEYS) {
        if (item?.[key] !== undefined && item?.[key] !== null) {
            return String(item[key]);
        }
    }

    return `Item ${index + 1}`;
}

export function renderDivergingBarChart(element, options) {
    if (!element || !options?.data?.length) {
        clearChart(element);
        return;
    }

    clearChart(element);

    const prepared = options.data.map((entry, index) => ({
        label: extractLabel(entry, index),
        value: Number(entry?.value ?? entry?.Value ?? 0)
    }));

    const {
        barHeight = 25,
        marginTop = 12,
        marginRight = 16,
        marginBottom = 30,
        marginLeft = 42,
        width = Math.max(element.clientWidth || 280, 280),
        metric = 'percent',
        positiveColor = '#4C9AFF',
        negativeColor = '#FF6B6B'
    } = options;

    const minValue = d3.min(prepared, d => d.value) ?? 0;
    const maxValue = d3.max(prepared, d => d.value) ?? 0;
    const domainMin = Math.min(minValue, 0);
    const domainMax = Math.max(maxValue, 0);
    const height = Math.ceil((prepared.length + 0.1) * barHeight) + marginTop + marginBottom;

    const x = d3.scaleLinear().domain([domainMin, domainMax]).nice().rangeRound([marginLeft, width - marginRight]);

    const y = d3
        .scaleBand()
        .domain(prepared.map(d => d.label))
        .rangeRound([marginTop, height - marginBottom])
        .padding(0.1);

    const format = d3.format(metric === 'absolute' ? '+,d' : '+.1%');
    const tickFormat = metric === 'absolute' ? d3.formatPrefix('+.1', 1e6) : d3.format('+.0%');

    const svg = d3
        .select(element)
        .append('svg')
        .attr('viewBox', [0, 0, width, height])
        .attr('preserveAspectRatio', 'none')
        .attr('width', width)
        .attr('height', height);

    svg
        .append('g')
        .selectAll('rect')
        .data(prepared)
        .join('rect')
        .attr('fill', d => (d.value >= 0 ? positiveColor : negativeColor))
        .attr('x', d => x(Math.min(d.value, 0)))
        .attr('y', d => y(d.label))
        .attr('width', d => Math.abs(x(d.value) - x(0)))
        .attr('height', y.bandwidth());

    svg
        .append('g')
        .attr('font-family', 'Montserrat')
        .attr('font-size', 10)
        .selectAll('text')
        .data(prepared)
        .join('text')
        .attr('text-anchor', d => (d.value < 0 ? 'end' : 'start'))
        .attr('x', d => x(d.value) + (d.value < 0 ? -4 : 4))
        .attr('y', d => (y(d.label) ?? marginTop) + y.bandwidth() / 2)
        .attr('dy', '0.35em')
        .text(d => format(d.value));

    svg
        .append('g')
        .attr('transform', `translate(0,${marginTop})`)
        .call(d3.axisTop(x).ticks(width / 80).tickFormat(tickFormat))
        .call(g =>
            g
                .selectAll('.tick line')
                .clone()
                .attr('y2', height - marginTop - marginBottom)
                .attr('stroke-opacity', 0.1)
        )
        .call(g => g.select('.domain').remove());

    svg
        .append('g')
        .attr('transform', `translate(${x(0)},0)`)
        .call(d3.axisLeft(y).tickSize(0).tickPadding(6))
        .call(g =>
            g
                .selectAll('.tick text')
                .filter((d, i) => prepared[i].value < 0)
                .attr('text-anchor', 'start')
                .attr('x', 6)
        );
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}