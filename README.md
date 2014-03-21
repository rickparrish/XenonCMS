XenonCMS
========

A minimalistic ASP.NET 4.5.1/MVC 5/EF 6/Bootstrap 3.1.1 based content management system<br />
<br />
I'm only putting this on GitHub so I have an easy way to push changes from dev to live -- I don't expect (or recommend) anybody else use it since I have no idea what I'm doing when it comes to building a CMS!<br />
<br />
But just in case you're feeling adventurous, don't forget to create a ConnectionStrings.config, since the Web.config file references it.  Here's what the dev version looks like:<br />
<br />
&lt;connectionStrings&gt;<br />
  &lt;add name=&quot;XenonCMSContext&quot; connectionString=&quot;Server=.\SQLEXPRESS;Database=XenonCMS-dev;Integrated Security=True;&quot; providerName=&quot;System.Data.SqlClient&quot; /&gt;<br />
&lt;/connectionStrings&gt;<br />
<br />
And why did I choose the name XenonCMS?  I'm glad you asked!<br />
<br />
My initial thought was to call it something like NIHCMS, short for Not Invented Here CMS (since pretty much the only reason I'm writing this is because I don't want to use somebody elses CMS, even if it is better and has more features and is less likely to crash, etc).  Pretty boring, right?<br />
So then I started looking into different acronyms, then anagrams, and eventually I found an interesting anagram for Not Built Here: Inert, Blue, Hot<br />
"Inert" caught my eye right away.  I sucked at chemistry, but I always liked the period table.  So immediately I went to look up the list of noble gases.<br />
"Blue" narrowed the list of candidate gases down nicely.  After some (very!) quick research, it looked like only Argon and Xenon fit.  And the picture for Argon mentioned something about mixing with Mercury to achieve the electric blue light, so I decided to go with Xenon instead<br />
"Hot" doesn't seem to fit, but who cares!
