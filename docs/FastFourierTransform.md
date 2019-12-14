[ソースコード](../CompLib/Algorithm/FastFourierTransform.cs)

$a,b$から$c_k=\sum_{i=0}^k a_i b_{k-i}$を$k=0,...,N+M$ですべて求める$(N=len(a)-1,M=len(b)-1)$

$a,b$を係数とする多項式
$$
    g(x)=
    \sum_{i=0}^{N}
    {
        a_ix^i
    } \\
    h(x)=
    \sum_{j=0}^{M}
    {
        b_jx^j
    }
$$
を考えると
$$
\begin{aligned}
    (g*h)(x)
    &=g(x)*h(x) \\
    &=\sum_{i=0}^{N}{\sum_{j=0}^{M}a_ib_jx^{i+j}}
\end{aligned}
$$

$i+j=k$とおいて
$$
\begin{aligned}
    (g*h)(x) 
    &=\sum_{k=0}^{N+M}{\sum_{i=0}^{k}a_ib_{k-i}x^k} \\
    &=\sum_{k=0}^{N+M}c_kx^k
\end{aligned}
$$
なので$(g*h)(x)$を求めたい

ナイーブな実装だと$O(NM)$かかる　遅い

ところで$(g*h)(x)$は$N+M$次の多項式なので

$N+M+1$個の点$x_i$での値$(g*h)(x_i)$があれば一意に定まる

なので
1. いい感じの$x_0,...,x_{n-1} (n \geq N+M+1)$を選ぶ
2. うまいことなんやかんやして$(g*h)(x)$を復元

すればいい
***
## 1. いい感じの$x_0,...,x_{n-1}$を選ぶ

$n$は2の冪乗

$\zeta_n=e^{2\pi \sqrt{-1}/n}$として$x_i=\zeta_n^i$とする

![zeta_n^i](/docs/imgs/a.png)

$\zeta_n$の性質として

* $\zeta_n^i=\zeta_n^j \Leftrightarrow {i \equiv j} \pmod{n}$
* $\zeta_n^i=-\zeta_n^{i+n/2}$
* $\overline{\zeta_n^i} = \zeta_n^{-i}$
*
$$
    \begin{aligned}
    \sum_{i=0}^{n-1} (\zeta_n^j)^i(\overline{\zeta_n^k})^i &=\sum_{i=0}^{n-1}\zeta_n^{i(j-k)} \\
    &=
    \begin{cases}
        n & \left({j \equiv k} \pmod{n}\right) \\
        0 & (otherwise)
    \end{cases}
    \end{aligned}
$$
また$\zeta_n$を$\zeta_n^{-1}$にしても上の性質を持つ  

***
## 2. うまいことなんやかんやして$(g*h)(x)$を復元

多項式$f(x)$に対し$\widehat{f}(t)$を
$$
    \widehat{f}(t)=
    \sum_{i=0}^{n-1}
    {
        f(\zeta_n^i)
        t^i
    }
$$
と定める  


$$
\begin{aligned}
    \widehat{f}(t)
    &=\sum_{i=0}^{n-1}
    {
        f(\zeta_n^i)
        t^i
    } \\
    &=\sum_{i=0}^{n-1}
    {
        \left(
            \sum_{j=0}^{n-1}
            {
                c_j
                \left(
                    \zeta_n^i
                \right)
            }
        ^j
        \right)
        t^i
    } \\
    &=\sum_{j=0}^{n-1}
    {
        c_j
        \sum_{i=0}^{n-1}
        {
            \left(
                \zeta_n^jt
            \right)
            ^i
        }
    }
\end{aligned}
$$

($f(x)$の離散フーリエ変換)

$\widehat{f}(\zeta_n^{-k})$を求めてみる

$$
\begin{aligned}
    \widehat{f}(\zeta_n^{-k})
    &=\sum_{j=0}^{n-1}
    {
        c_j
        \sum_{i=0}^{n-1}
        {
            \left(
                \zeta_n^j
                \zeta_n^{-k}
            \right)
        }
        ^i
    } \\
    &=\sum_{j=0}^{n-1}
    {
        c_j
        \sum_{i=0}^{n-1}
        {
            \zeta_n^{i(j-k)}
        }
    }
    \\
    &=nc_k
\end{aligned}
$$

よって

$$
    f(x)=
    \frac{1}{n}
    \sum_{i=0}^{n-1}
    {
        \widehat{f}
        (
            \zeta_n^{-i}
        )
        x^i
    }
$$

${\zeta_n}$が${\zeta_n^{-1}}$に置き換わった離散フーリエ変換で$\widehat{f}(x)$から$f(x)$に復元できる

(離散フーリエ逆変換)
