import * as d3 from 'https://cdn.jsdelivr.net/npm/d3@7/+esm';

const ratingStops = [
    { val: 0, color: '#CD0000' },
    { val: 3000, color: '#FF6347' },
    { val: 5500, color: '#CDCD27' },
    { val: 6500, color: '#27CD27' },
    { val: 7200, color: '#2EAD69' },
    { val: 8000, color: '#00ADAD' },
    { val: 10000, color: '#9500FF' }
];

function clamp(value, min, max) {
    return Math.max(min, Math.min(max, value));
}

function hexToRgb(hex) {
    const value = hex.replace('#', '');
    const normalized = value.length === 3
        ? value.split('').map(char => char + char).join('')
        : value;

    const intValue = Number.parseInt(normalized, 16);
    return {
        r: (intValue >> 16) & 255,
        g: (intValue >> 8) & 255,
        b: intValue & 255
    };
}

function interpolateColor(aHex, bHex, t) {
    const a = hexToRgb(aHex);
    const b = hexToRgb(bHex);
    const safeT = clamp(t, 0, 1);

    const r = Math.round(a.r + (b.r - a.r) * safeT);
    const g = Math.round(a.g + (b.g - a.g) * safeT);
    const bValue = Math.round(a.b + (b.b - a.b) * safeT);

    return `rgb(${r}, ${g}, ${bValue})`;
}

function getColorForRating(rating) {
    if (rating === null || rating === undefined || Number.isNaN(Number(rating))) {
        return '#77777777';
    }

    const normalized = clamp(Number(rating), 0, 10000);

    for (let i = 0; i < ratingStops.length - 1; i += 1) {
        const a = ratingStops[i];
        const b = ratingStops[i + 1];

        if (normalized >= a.val && normalized <= b.val) {
            const t = (normalized - a.val) / (b.val - a.val);
            return interpolateColor(a.color, b.color, t);
        }
    }

    return ratingStops[ratingStops.length - 1].color;
}

export function renderAlbumTimelineChart(element, options) {
    if (!element || !options?.albums?.length) {
        clearChart(element);
        return;
    }

    const {
        albums,
        height = 360,
        marginTop = 24,
        marginRight = 24,
        marginBottom = 42,
        marginLeft = 24,
        yearTickInterval = 5,
        dotNetRef = null,
        callbackMethodName = 'NotifyAlbumClicked'
    } = options;

    clearChart(element);

    const width = Math.max(element.clientWidth || 860, 420);
    const safeInterval = Math.max(1, Number(yearTickInterval) || 1);

    const parsedAlbums = albums
        .map(album => ({
            id: album.id,
            name: album.name ?? 'Unknown album',
            artworkUrl: album.artworkUrl,
            rating: clamp(Number(album.rating ?? 0), 0, 10000),
            releaseDate: new Date(album.releaseDate)
        }))
        .filter(album => !Number.isNaN(album.releaseDate.getTime()))
        .sort((a, b) => a.releaseDate - b.releaseDate || a.id - b.id);

    if (!parsedAlbums.length) {
        return;
    }

    const minYear = d3.min(parsedAlbums, album => album.releaseDate.getFullYear()) ?? new Date().getFullYear();
    const maxYear = d3.max(parsedAlbums, album => album.releaseDate.getFullYear()) ?? minYear;

    const xScale = d3
        .scaleLinear()
        .domain([minYear, maxYear === minYear ? minYear + 1 : maxYear])
        .range([marginLeft, width - marginRight]);

    const yScale = d3
        .scaleLinear()
        .domain([0, 10000])
        .range([height - marginBottom, marginTop]);

    const yearTicks = d3
        .range(Math.floor(minYear / safeInterval) * safeInterval, maxYear + safeInterval + 1, safeInterval)
        .filter(year => year >= minYear && year <= maxYear);

    const svg = d3
        .select(element)
        .append('svg')
        .attr('width', width)
        .attr('height', height)
        .attr('viewBox', `0 0 ${width} ${height}`)
        .attr('preserveAspectRatio', 'none');

    const defs = svg.append('defs');
    const gradient = defs
        .append('linearGradient')
        .attr('id', 'album-timeline-line-gradient')
        .attr('gradientUnits', 'userSpaceOnUse')
        .attr('x1', 0)
        .attr('x2', 0)
        .attr('y1', yScale(0))
        .attr('y2', yScale(10000));

    ratingStops.forEach(stop => {
        const offset = (stop.val / 10000.0) * 100.0;
        gradient
            .append('stop')
            .attr('offset', `${offset}%`)
            .attr('stop-color', stop.color);
    });

    const xAxis = d3.axisBottom(xScale)
        .tickValues(yearTicks.length ? yearTicks : [minYear, maxYear])
        .tickFormat(d3.format('d'));

    svg
        .append('g')
        .attr('transform', `translate(0, ${height - marginBottom})`)
        .call(xAxis)
        .call(group => group.select('.domain').attr('stroke', 'rgba(255,255,255,0.25)'))
        .call(group => group.selectAll('.tick line').attr('stroke', 'rgba(255,255,255,0.18)'))
        .call(group => group.selectAll('.tick text').attr('fill', 'rgba(255,255,255,0.82)').style('font-size', '11px'));

    svg
        .append('g')
        .attr('transform', `translate(${marginLeft},0)`)
        .call(d3.axisLeft(yScale).ticks(5).tickSize(-(width - marginLeft - marginRight)).tickFormat(() => ''))
        .call(group => group.select('.domain').remove())
        .call(group => group.selectAll('.tick line').attr('stroke', 'rgba(255,255,255,0.14)'))
        .call(group => group.selectAll('.tick text').remove());

    const lineGenerator = d3
        .line()
        .x(album => xScale(album.releaseDate.getFullYear()))
        .y(album => yScale(album.rating))
        .curve(d3.curveCatmullRom.alpha(0.65));

    svg
        .append('path')
        .datum(parsedAlbums)
        .attr('fill', 'none')
        .attr('stroke', 'url(#album-timeline-line-gradient)')
        .attr('stroke-width', 8)
        .attr('stroke-linecap', 'round')
        .attr('stroke-linejoin', 'round')
        .attr('opacity', 0.95)
        .attr('d', lineGenerator);

    const tooltip = d3
        .select(element)
        .append('div')
        .attr('class', 'album-timeline-tooltip')
        .style('position', 'absolute')
        .style('pointer-events', 'none')
        .style('opacity', 0);

    const itemSize = 44;

    const points = svg
        .append('g')
        .selectAll('g.album-point')
        .data(parsedAlbums)
        .join('g')
        .attr('class', 'album-point')
        .attr('transform', album => `translate(${xScale(album.releaseDate.getFullYear())}, ${yScale(album.rating)})`)
        .style('cursor', 'pointer');

    points
        .append('circle')
        .attr('r', itemSize * 0.62)
        .attr('fill', album => getColorForRating(album.rating))
        .attr('fill-opacity', 0.18)
        .attr('stroke', album => getColorForRating(album.rating))
        .attr('stroke-opacity', 0.6)
        .attr('stroke-width', 1.8);

    points.each(function appendCover(album, index) {
        const group = d3.select(this);
        const clipId = `album-cover-clip-${index}-${album.id}`;

        defs
            .append('clipPath')
            .attr('id', clipId)
            .append('rect')
            .attr('x', -itemSize / 2)
            .attr('y', -itemSize / 2)
            .attr('width', itemSize)
            .attr('height', itemSize)
            .attr('rx', 10)
            .attr('ry', 10);

        group
            .append('image')
            .attr('href', album.artworkUrl)
            .attr('x', -itemSize / 2)
            .attr('y', -itemSize / 2)
            .attr('width', itemSize)
            .attr('height', itemSize)
            .attr('clip-path', `url(#${clipId})`)
            .attr('preserveAspectRatio', 'xMidYMid slice');
    });

    points
        .on('mouseenter', function onEnter(event, album) {
            d3.select(this)
                .raise()
                .transition()
                .duration(180)
                .attr('transform', `translate(${xScale(album.releaseDate.getFullYear())}, ${yScale(album.rating)}) scale(1.22)`);

            tooltip
                .html(`<div class="album-timeline-tooltip__title">${album.name}</div><div class="album-timeline-tooltip__rating">${Math.round(album.rating)}</div>`)
                .style('opacity', 1)
                .style('left', `${event.offsetX + 16}px`)
                .style('top', `${event.offsetY - 10}px`);
        })
        .on('mousemove', (event) => {
            tooltip
                .style('left', `${event.offsetX + 16}px`)
                .style('top', `${event.offsetY - 10}px`);
        })
        .on('mouseleave', function onLeave(event, album) {
            d3.select(this)
                .transition()
                .duration(180)
                .attr('transform', `translate(${xScale(album.releaseDate.getFullYear())}, ${yScale(album.rating)}) scale(1)`);

            tooltip.style('opacity', 0);
        })
        .on('click', (_, album) => {
            if (dotNetRef?.invokeMethodAsync) {
                dotNetRef.invokeMethodAsync(callbackMethodName, Number(album.id));
            }
        });
}

export function clearChart(element) {
    if (element) {
        element.innerHTML = '';
    }
}