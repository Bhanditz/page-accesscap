# Page-access Limiter a.k.a. page-accesscap
I recently needed to limit access frequency to a specific page/web method when the visit count hits certain threshold, so I came up with this approach of caching the hit count and implementing an `IActionFilter` on a `FilterAttribute` to intercept the requests and act on them accordingly.

I believe this is also one known step to overcome the infamous [DDoS attack](https://en.wikipedia.org/wiki/Denial-of-service_attack). Now this approach I'm talking about handles two types of user:
* potentially malicious users
* regular users constantly hitting certain page or invoking certain method (with no bad intention of course)

When it is the former one, the way I like to handle this is I will redirect the bad guy to some external [warning page](https://legalpiracy.wordpress.com/2011/01/10/ddos-attacks-and-the-law) and drive them out at once! Otherwise, prompt them to come back later after some coffee break.
