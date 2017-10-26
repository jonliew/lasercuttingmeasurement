pathLength = 0;
for i=1:size(Q,2)-1
    x1 = Q(1,i);
    y1 = Q(2,i);
    x2 = Q(1,i+1);
    y2 = Q(2,i+1);
    pathLength = pathLength + sqrt((x1-x2)^2+(y1-y2)^2);
end
pathLength