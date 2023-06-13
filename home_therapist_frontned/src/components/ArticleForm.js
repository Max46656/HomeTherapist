import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import { LayoutMarTop } from "./style";

const PAGE_SIZE = 12;  // 每頁的文章數量

const ArticleForm = () => {
  const [articles, setArticles] = useState([]);
  const [currentPage, setCurrentPage] = useState(0);  // 當前頁數

  const fetchArticles = () => {
    axios
      .get("https://localhost:5000/api/Articles")
      .then((res) => {
        const data = res.data.data;
        setArticles(data);
      })
      .catch((err) => console.log(err));
  };

  useEffect(() => {
    fetchArticles();
  }, []);

  const handlePreviousPage = () => {
    setCurrentPage(Math.max(currentPage - 1, 0));
  };

  const handleNextPage = () => {
    setCurrentPage(Math.min(currentPage + 1, Math.ceil(articles.length / PAGE_SIZE) - 1));
  };

  const currentArticles = articles.slice(currentPage * PAGE_SIZE, (currentPage + 1) * PAGE_SIZE);

  return (
    <div>
      <LayoutMarTop />
      <div className="container py-md-5 article_style">
        <h1 className="mb-md-5 article_style">文章</h1>
        <div>
          <table className="table">
            <thead>
              <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Subtitle</th>
              </tr>
            </thead>
            <tbody>
              {currentArticles.map((article) => (
                <tr key={article.id}>
                  <th scope="row">{article.id}</th>
                  <td className="col-4 article_font">{article.title}</td>
                  <td className="col-4 article_font">{article.subtitle}</td>
                  <td>
                    <Link to={`/Article/${article.id}`}>查看</Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <button onClick={handlePreviousPage}>上一頁</button>
        <button onClick={handleNextPage}>下一頁</button>
      </div>
    </div>
  );
};

export default ArticleForm;
