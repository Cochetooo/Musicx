window.d3Charts = {
    renderDivergingBarChart: (element, options) => {
        if (!element || !window.d3 || !options?.data?.length) {
            if (element) {
                element.innerHTML = '';
            }
            return;
        }
        
        const d3 = window.d3;
        const {
            data,
            barHeight = 25,
            marginTop = 12,
            marginRight = 16,
            marginBottom = 30,
            marginLeft = 42,
            width = 220,
            height = Math.ceil((data.length + 0.1) * barHeight) + marginTop + marginBottom
        } = options;
        
        const x = d3.scaleLinear()
            .domain(d3.extent(data, d => d.value))
            .rangeRound([marginLeft, width - marginRight]);
        
        const y = d3.scaleBand()
            .domain(data.map(d => d.Label))
            .padding(0.1);
        
        const format = d3.format(metric === "absolute" ? "+,d" : "+.1%");
        const tickFormat = metric === "absolute" ? d3.formatPrefix("+.1", 1e6) : d3.format("+.0%");
        
        const svg = d3
            .select(element)
            .append("svg")
            .attr("viewBox", [0, 0, width, height])
            .attr("preserveAspectRatio", "none");
        
        svg.append("g")
            .selectAll()
            .data(data)
            .join("rect")
            .attr("fill", (d) => d3.schemeRdBu[3][d.value > 0 ? 2 : 0])
            .attr("x", (d) => x(Math.min(d.value, 0)))
            .attr("y", (d) => y(d.Label))
            .attr("width", d => Math.abs(x(d.value) - x(0)))
            .attr("height", y.bandwidth());
        
        svg.append("g")
            .attr("font-family", "Montserrat")
            .attr("font-size", 10)
            .selectAll()
            .data(data)
            .join("text")
            .attr("text-anchor", d => d.value < 0 ? "end" : "start")
            .attr("x", (d) => x(d.value) + Math.sign(d.value - 0) * 4)
            .attr("y", (d) => y(d.State) + y.bandwidth() / 2)
            .attr("dy", "0.35em")
            .text(d => format(d.value));

        svg.append("g")
            .attr("transform", `translate(0,${marginTop})`)
            .call(d3.axisTop(x).ticks(width / 80).tickFormat(tickFormat))
            .call(g => g.selectAll(".tick line").clone()
                .attr("y2", height - marginTop - marginBottom)
                .attr("stroke-opacity", 0.1))
            .call(g => g.select(".domain").remove());

        svg.append("g")
            .attr("transform", `translate(${x(0)},0)`)
            .call(d3.axisLeft(y).tickSize(0).tickPadding(6))
            .call(g => g.selectAll(".tick text").filter((d, i) => data[i].value < 0)
                .attr("text-anchor", "start")
                .attr("x", 6));
        
        return svg.node();
    },
    
    renderLineChart: (element, options) => {
        if (!element || !window.d3 || !options?.points?.length) {
            if (element) {
                element.innerHTML = '';
            }
            return;
        }

        const d3 = window.d3;
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
        element.innerHTML = '';

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
            .call(
                d3
                    .axisBottom(xScale)
                    .tickSize(0)
            )
            .call(group => group.select('.domain').remove())
            .call(group => group.selectAll('text').attr('fill', tickColor).style('font-size', '11px'));

        svg
            .append('g')
            .attr('transform', `translate(${marginLeft},0)`)
            .call(
                d3
                    .axisLeft(yScale)
                    .ticks(5)
                    .tickSize(-(width - marginLeft - marginRight))
            )
            .call(group => group.select('.domain').remove())
            .call(group => group.selectAll('.tick line').attr('stroke', gridColor))
            .call(group => group.selectAll('.tick text').attr('fill', tickColor).style('font-size', '11px'));

        svg
            .append('path')
            .datum(points)
            .attr('fill', fillColor)
            .attr('d', areaGenerator);

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
    },

    clearChart: element => {
        if (element) {
            element.innerHTML = '';
        }
    }
};