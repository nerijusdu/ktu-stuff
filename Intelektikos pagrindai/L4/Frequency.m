function p = Frequency(filename, map, n, delimiters)
    defaultVal = 0.1;
    f = fileread(char(filename));
    C = lower(strsplit(f,delimiters))';
    uniq = unique(C);
    prob = zeros(length(uniq),1);
    bool = isKey(map,uniq);

    for i = 1:length(uniq)
        if bool(i) == 1, prob(i) = map(uniq{i});
        else, prob(i) = defaultVal; end
    end

    prob = sort(prob);

    if length(prob) < 2 * n
        n = floor(length(prob) / 2);
    end
        top = prod(prob(1:n)) * prod(prob(length(prob)-n+1:length(prob)));
        bottom = prod(prob(1:n)) + prod(1 - prob(length(prob)-n+1:length(prob)));
        p = top / bottom;
    end
    