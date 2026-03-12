import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

/**
 * data: [{ genre, entries:[{ year, value, subGenres:[{name, year, value}] }] }]
 */
export function renderGenreArchitectureChart(element, options) {
    if (!element || !options?.data?.length) {
        clearChart(element);
        return;
    }

    const {
        data,
        width = Math.max(element.clientWidth || 960, 420),
        height = 520,
        margin = { top: 24, right: 24, bottom: 42, left: 82 },
        dotNetRef = null,
        callbackMethodName = 'OnSubGenreClicked'
    } = options;

    clearChart(element);

    const bars = data.flatMap(genre =>
        (genre.entries ?? []).map(entry => ({
            genre: genre.genre,
            year: Number(entry.year),
            value: Number(entry.value ?? 0),
            subGenres: entry.subGenres ?? []
        }))
    );

    const years = [...new Set(bars.map(b => b.year))].sort((a, b) => a - b);
    const genres = [...new Set(bars.map(b => b.genre))];

    const x = d3.scaleBand().domain(genres).range([margin.left, width - margin.right]).paddingInner(0.1);
    const xSub = d3.scaleBand().domain(years).range([0, x.bandwidth()]).padding(0.06);
    const y = d3.scaleLinear().domain([0, d3.max(bars, b => b.value) ?? 1]).nice().range([height - margin.bottom, margin.top]);
    const color = d3.scaleSequential().domain(d3.extent(years)).interpolator(d3.interpolateTurbo);

    const svg = d3
        .select(element)
        .append('svg')
        .attr('viewBox', [0, 0, width, height])
        .style('max-width', '100%');

    const defs = svg.append('defs');
    const gradient = defs
        .append('linearGradient')
        .attr('id', 'genreBarGradient')
        .attr('x1', '0%')
        .attr('x2', '0%')
        .attr('y1', '100%')
        .attr('y2', '0%');
    gradient.append('stop').attr('offset', '0%').attr('stop-color', 'white').attr('stop-opacity', 0.4);
    gradient.append('stop').attr('offset', '100%').attr('stop-color', 'white').attr('stop-opacity', 0);

    const root = svg.append('g');

    const genreGroups = root
        .selectAll('g.genre')
        .data(genres)
        .join('g')
        .attr('class', 'genre')
        .attr('transform', genre => `translate(${x(genre)},0)`);

    genreGroups
        .selectAll('rect.year-bar')
        .data(genre => bars.filter(bar => bar.genre === genre))
        .join('rect')
        .attr('class', 'year-bar')
        .attr('x', d => xSub(d.year))
        .attr('y', d => y(d.value))
        .attr('width', xSub.bandwidth())
        .attr('height', d => y(0) - y(d.value))
        .attr('fill', d => color(d.year))
        .attr('fill-opacity', 0.66)
        .attr('stroke', 'rgba(255,255,255,0.08)')
        .append('title')
        .text(d => `${d.genre} (${d.year})\n${d.value}`);

    genreGroups
        .selectAll('rect.overlay')
        .data(genre => bars.filter(bar => bar.genre === genre))
        .join('rect')
        .attr('class', 'overlay')
        .attr('x', d => xSub(d.year))
        .attr('y', d => y(d.value))
        .attr('width', xSub.bandwidth())
        .attr('height', d => y(0) - y(d.value))
        .attr('fill', 'url(#genreBarGradient)');

    const genreLabels = genreGroups
        .append('text')
        .attr('x', x.bandwidth() / 2)
        .attr('y', height - margin.bottom - 4)
        .attr('transform', `rotate(-90, ${x.bandwidth() / 2}, ${height - margin.bottom - 4})`)
        .attr('text-anchor', 'end')
        .attr('fill', 'currentColor')
        .text(genre => genre);

    const subLayer = genreGroups.append('g').attr('class', 'sub-genre-layer').attr('opacity', 0);

    subLayer
        .selectAll('text')
        .data(genre => bars.filter(bar => bar.genre === genre).flatMap(bar => bar.subGenres.map(sub => ({ ...sub, hostYear: bar.year }))))
        .join('text')
        .attr('x', d => (xSub(d.year ?? d.hostYear) ?? 0) + xSub.bandwidth() / 2)
        .attr('y', d => y(Number(d.value ?? 0)) - 6)
        .attr('text-anchor', 'middle')
        .attr('font-size', 10)
        .attr('fill', 'currentColor')
        .style('cursor', 'pointer')
        .text(d => d.name)
        .on('click', (_, d) => {
            if (dotNetRef?.invokeMethodAsync) {
                dotNetRef.invokeMethodAsync(callbackMethodName, d.name, d.year ?? d.hostYear);
            }
        });

    svg
        .call(
            d3
                .zoom()
                .scaleExtent([1, 8])
                .translateExtent([
                    [0, 0],
                    [width, height]
                ])
                .on('zoom', ({ transform }) => {
                    root.attr('transform', transform);
                    const showSubGenres = transform.k > 2.2;
                    genreLabels.attr('opacity', showSubGenres ? 0 : 1);
                    subLayer.attr('opacity', showSubGenres ? 1 : 0);
                })
        )
        .on('dblclick.zoom', null);

    root.append('g').attr('transform', `translate(0,${height - margin.bottom})`).call(d3.axisBottom(x).tickFormat(() => '')).call(g => g.select('.domain').attr('opacity', 0.35));

    root.append('g').attr('transform', `translate(${margin.left},0)`).call(d3.axisLeft(y).ticks(5));
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}