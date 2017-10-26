majorEndX = 0;
majorEndY = 0.513191736507765;
minorRatio = 0.5525
startParameter = 0.0;
endParameter = 4.69484422064888;

major = sqrt((majorEndX)^2+(majorEndY)^2);
minor = major * minorRatio;

step = 0.0001;
oldX = major * cos(startParameter);
oldY = minor * sin(startParameter);
length = 0;
for t=startParameter+step:step:endParameter
    x = major*cos(t);
    y = minor*sin(t);
    length = length + sqrt((oldX-x)^2+(oldY-y)^2);
    oldX = x;
    oldY = y;
end
length